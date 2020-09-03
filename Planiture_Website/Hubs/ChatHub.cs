using Microsoft.AspNetCore.SignalR;
using Planiture_Website.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Planiture_Website.Hubs
{
    public class ChatHub : Hub
    {
        public ApplicationDbContext _context;
        private static ConcurrentDictionary<string, Agent> _agents;
        private static ConcurrentDictionary<string, string> _chatSessions;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgentConnect(string name, string pass)
        {
            if (_agents == null)
                _agents = new ConcurrentDictionary<string, Agent>();

            if (_chatSessions == null)
                _chatSessions = new ConcurrentDictionary<string, string>();

            string hashPass = ToHash(pass);
            var test = _context.ConfigFiles.ToList();

            SqlConnection con = new SqlConnection(@"Data Source=MSI;Initial Catalog=LiveChat;Integrated Security=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("select AdminPass, AgentPass from ConfigFiles", con);
            List<string> str = new List<string>();
            SqlDataReader da = cmd.ExecuteReader();
            while (da.Read())
            {
                str.Add(da.GetValue(0).ToString());
                str.Add(da.GetValue(1).ToString());
            }
            con.Close();


            if (test == null)
            {
                await Clients.All.SendAsync("LoginResult", false, "config", "");
            }
            else if ((str[0] == hashPass) || (str[1] == hashPass))
            {
                var agent = new Agent
                {
                    Id = Context.ConnectionId,
                    Name = name,
                    IsOnline = true
                };

                // if the agent is already signed-in
                if (_agents.Any(x => x.Key == name))
                {
                    agent = _agents[name];

                    await Clients.Caller.SendAsync("LoginResult", true, agent.Id, agent.Name);

                    await Clients.Caller.SendAsync("OnlineStatus", _agents.Count(x => x.Value.IsOnline) > 0);
                }
                else if (_agents.TryAdd(name, agent))
                {
                    await Clients.Caller.SendAsync("LoginResult", true, agent.Id, agent.Name);

                    await Clients.Caller.SendAsync("OnlineStatus", _agents.Count(x => x.Value.IsOnline) > 0);
                }
                else
                {
                    await Clients.Caller.SendAsync("LoginResult", false, "error", "");
                }
            }
            else
                await Clients.Caller.SendAsync("LoginResult", false, "pass", "");


        }

        public void ChangeStatus(bool online)
        {
            var agent = _agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;
            if (agent != null)
            {
                agent.IsOnline = online;
                // TODO: Check if the agent was in chat sessions.
                Clients.All.SendAsync("OnlineStatus", _agents.Count(x => x.Value.IsOnline) > 0);
            }

        }

        public void EngageVisitor(string connectionId)
        {
            var agent = _agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;

            if (agent != null && _chatSessions.Count(x => x.Key == connectionId && x.Value == agent.Id) == 0)
            {
                _chatSessions.TryAdd(connectionId, agent.Id);
                Clients.Caller.SendAsync("NewChat", connectionId);
                Clients.Client(connectionId).SendAsync("SetChat", connectionId, agent.Name, false);
                Clients.Client(connectionId).SendAsync("OpenChatWindow");
                Clients.Caller.SendAsync("AddMessage", connectionId, "system", "You invited this visitor to chat...");
                Clients.Client(connectionId).SendAsync("AddMessage", agent.Name, "Hey there. I'm " + agent.Name + " let me know if you have any questions.");


            }

        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void RequestChat(string message)
        {
            //we assign the chat to the kingz agent
            var workload = from a in _agents
                           where a.Value.IsOnline
                           select new
                           {
                               a.Value.Id,
                               a.Value.Name,
                               Count = _chatSessions.Count(x => x.Value == a.Value.Id)

                           };
            var kingz = workload.OrderBy(x => x.Count).FirstOrDefault();
            if (kingz == null)
            {
                Clients.Caller.SendAsync("AddMessage", "", "No agent are currently available.");
                return;
            }

            _chatSessions.TryAdd(Context.ConnectionId, kingz.Id);
            Clients.Client(kingz.Id).SendAsync("NewChat", Context.ConnectionId);
            Clients.Caller.SendAsync("SetChat", Context.ConnectionId, kingz.Name);
            message = Regex.Replace(message, @"(\b(?:(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&@#/%?=~_|$!:,.;]*[-A-Z0-9+&@#/%=~_|$]|((?:mailto:)?[A-Z0-9._%+-]+@[A-Z0-9._%-]+\.[A-Z]{2,6})\b)|""(?:(?:https?|ftp|file)://|www\.|ftp\.)[^""\r\n]+""|'(?:(?:https?|ftp|file)://|www\.|ftp\.)[^'\r\n]+')", "<a target='_blank' href='$1'>$1</a>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Clients.Client(kingz.Id).SendAsync("AddMessage", Context.ConnectionId, "visitor", message);
            Clients.Caller.SendAsync("AddMessage", "me", message);


        }

        public void LogVisit(string page, string referrer, string city, string region, string country, string existingChatId)
        {
            if (_agents == null)
                _agents = new ConcurrentDictionary<string, Agent>();

            Clients.Caller.SendAsync("OnlineStatus", _agents.Count(x => x.Value.IsOnline) > 0);
            var cityDisplayName = GetCityDisplayName(city, region);
            var countryDisplayName = country ?? string.Empty;

            if (!string.IsNullOrEmpty(existingChatId) &&
                _chatSessions.ContainsKey(existingChatId))
            {
                var agentId = _chatSessions[existingChatId];
                Clients.Client(agentId).SendAsync("VisitorSwitchPage", existingChatId, Context.ConnectionId, page);
                var agent = _agents.SingleOrDefault(x => x.Value.Id == agentId).Value;

                if (agent != null)
                    Clients.Caller.SendAsync("SetChat", Context.ConnectionId, agent.Name, true);

                string buffer;
                _chatSessions.TryRemove(existingChatId, out buffer);
                _chatSessions.TryAdd(Context.ConnectionId, agentId);

            }

            foreach (var agent in _agents)
            {
                var chatWith = (from c in _chatSessions
                                join a in _agents on c.Value equals a.Value.Id
                                where c.Key == Context.ConnectionId
                                select a.Value.Name).SingleOrDefault();

                Clients.Client(agent.Value.Id).SendAsync("NewVisit", page, referrer, cityDisplayName, countryDisplayName, chatWith, Context.ConnectionId);
            }
        }

        private string GetCityDisplayName(string city, string region)
        {
            var displayCity = string.Empty;
            if (!string.IsNullOrEmpty(city))
            {
                displayCity = city;
                if (!string.IsNullOrEmpty(region))
                {
                    displayCity += ", " + region;
                }
            }
            return displayCity;
        }

        public string ToHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            var provider = new SHA1CryptoServiceProvider();
            var encoding = new UnicodeEncoding();
            return Convert.ToBase64String(provider.ComputeHash(encoding.GetBytes(password)));
        }

        public void Transfer(string connectionId, string agentName, string messages)
        {
            if (!_agents.ContainsKey(agentName))
            {
                Clients.Caller.SendAsync("AddMessage", Context.ConnectionId, "system", "This agent does not exists: " + agentName);
                return;
            }

            var agent = _agents[agentName];
            if (!agent.IsOnline)
            {
                Clients.Caller.SendAsync("AddMessage", Context.ConnectionId, "system", agentName + " is not online at the moment.");
                return;
            }

            if (!_chatSessions.ContainsKey(connectionId))
            {
                Clients.Caller.SendAsync("AddMessage", Context.ConnectionId, "system", "This chat session does not exists anymore.");
                return;
            }

            string currentAgentId;
            if (_chatSessions.TryRemove(connectionId, out currentAgentId) &&
                _chatSessions.TryAdd(connectionId, agent.Id))
            {
                Clients.Client(agent.Id).SendAsync("NewChat", connectionId);
                Clients.Client(agent.Id).SendAsync("AddMessage", connectionId, "system", "New chat transfered to you.");
                Clients.Client(agent.Id).SendAsync("AddMessage", connectionId, ">>", "Starting previous conversation");
                Clients.Client(agent.Id).SendAsync("AddMessage", "", messages);
                Clients.Client(agent.Id).SendAsync("AddMessage", connectionId, "<<", "End of previous conversation");

                Clients.Client(connectionId).SendAsync("AddMessage", "", "You have been transfered to " + agent.Name);
                Clients.Client(connectionId).SendAsync("SetChat", connectionId, agent.Name, true);

                Clients.Caller.SendAsync("AddMessage", connectionId, "system", "Chat transfered to " + agentName);
            }
        }

        public void Send(string data)
        {
            //snatch any url using regex pattern
            data = Regex.Replace(data, @"(\b(?:(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&@#/%?=~_|$!:,.;]*[-A-Z0-9+&@#/%=~_|$]|((?:mailto:)?[A-Z0-9._%+-]+@[A-Z0-9._%-]+\.[A-Z]{2,6})\b)|""(?:(?:https?|ftp|file)://|www\.|ftp\.)[^""\r\n]+""|'(?:(?:https?|ftp|file)://|www\.|ftp\.)[^'\r\n]+')", "<a target='_blank' href='$1'>$1</a>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Clients.Caller.SendAsync("AddMessage", "me", data);

            if (_chatSessions.ContainsKey(Context.ConnectionId))
            {
                var opId = _chatSessions[Context.ConnectionId];
                Clients.Client(opId).SendAsync("AddMessage", Context.ConnectionId, "visitor", data);

            }
            else
            {
                Debug.WriteLine("Chat Session not found.");

                //refactor this
                var workload = from a in _agents
                               where a.Value.IsOnline
                               select new
                               {
                                   a.Value.Id,
                                   a.Value.Name,
                                   Count = _chatSessions.Count(x => x.Value == a.Value.Id)
                               };
                var kingz = workload.OrderBy(x => x.Count).FirstOrDefault();
                if (kingz == null)
                {
                    Clients.Caller.SendAsync("AddMessage", "", "No agent are currently available.");
                    return;
                }
                _chatSessions.TryAdd(Context.ConnectionId, kingz.Id);
                Clients.Client(kingz.Id).SendAsync("NewChat", Context.ConnectionId);
                Clients.Caller.SendAsync("SetChat", Context.ConnectionId, kingz.Name, false);
                Clients.Client(kingz.Id).SendAsync("AddMessage", Context.ConnectionId, "system", "This visitor appear to have lost their chat session.");
                Clients.Client(kingz.Id).SendAsync("AddMessage", Context.ConnectionId, "visitor", data);

            }

        }

        public void OpSend(string id, string data)
        {
            var agent = _agents.SingleOrDefault(x => x.Value.Id == Context.ConnectionId).Value;
            if (agent == null)
            {
                Clients.Caller.SendAsync("AddMessage", id, "system", "We were unable to send your message, please reload the page.");
                return;
            }

            if (id == "internal")
            {
                foreach (var a in _agents.Where(x => x.Value.IsOnline))
                    Clients.Client(a.Value.Id).SendAsync("AddMessage", id, agent.Name, data);

            }
            else if (_chatSessions.ContainsKey(id))
            {
                data = Regex.Replace(data, @"(\b(?:(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&@#/%?=~_|$!:,.;]*[-A-Z0-9+&@#/%=~_|$]|((?:mailto:)?[A-Z0-9._%+-]+@[A-Z0-9._%-]+\.[A-Z]{2,6})\b)|""(?:(?:https?|ftp|file)://|www\.|ftp\.)[^""\r\n]+""|'(?:(?:https?|ftp|file)://|www\.|ftp\.)[^'\r\n]+')", "<a target='_blank' href='$1'>$1</a>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Clients.Caller.SendAsync("AddMessage", id, "you", data);
                Clients.Client(id).SendAsync("AddMessage", agent.Name, data);
            }
        }

        public void CloseChat(string id)
        {
            if (_chatSessions.ContainsKey(id))
            {
                Clients.Client(id).SendAsync("AddMessage", "", "The agent close the chat session.");

                string buffer;
                _chatSessions.TryRemove(id, out buffer);
            }
        }

        public void LeaveChat(string id)
        {
            // was it an agent
            var agent = _agents.SingleOrDefault(x => x.Value.Id == id).Value;
            if (agent != null)
            {
                Agent tmp;
                if (_agents.TryRemove(agent.Name, out tmp))
                {
                    var sessions = _chatSessions.Where(x => x.Value == agent.Id);
                    foreach (var session in sessions)
                        Clients.Client(session.Key).SendAsync("AddMessage", "", "The agent was disconnected from chat.");
                    Clients.All.SendAsync("UpdateStatus", _agents.Count(x => x.Value.IsOnline) > 0);
                }
            }

            // was it a visitor
            if (_chatSessions.ContainsKey(id))
            {
                var agentId = _chatSessions[id];
                Clients.Client(agentId).SendAsync("AddMessage", id, "system", "The visitor close the connection.");
            }
        }

        public Task OnDisconnected(bool stopCalled)
        {
            return Clients.All.SendAsync("Leave", Context.ConnectionId);
        }

        public void SendEmail(string from, string message)
        {
            var test = _context.ConfigFiles.ToList();
            if (test != null && test.Count >= 3)
            {
                message = "From: " + from + "\n\n" + message;

                SqlConnection con = new SqlConnection(@"Data Source=MSI;Initial Catalog=LiveChat;Integrated Security=True");
                con.Open();
                SqlCommand cmd = new SqlCommand("select Email from ConfigFiles", con);
                List<string> str = new List<string>();
                SqlDataReader da = cmd.ExecuteReader();
                while (da.Read())
                {
                    str.Add(da.GetValue(0).ToString());
                }
                con.Close();

                var msg = new MailMessage();
                msg.From = new MailAddress(str[0]);
                msg.To.Add(new MailAddress(str[0]));
                msg.Subject = "LCSK - Offline Contact";
                msg.Body = "You received an offline contact from your LCSK chat widget.\r\n\r\n" + message;

                using (var client = new SmtpClient())
                {
                    client.Send(msg);
                }
            }
        }




    }
}
