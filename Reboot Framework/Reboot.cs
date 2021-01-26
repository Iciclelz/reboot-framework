/*
    Copyright (C) 2016 Ryukuo

    This file is part of Reboot Framework.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Reboot_Framework
{
    public class Reboot
    {
        private List<Tuple<int, string>> scripts;
        private Engine cheatEngine;
        private bool attached;
        public Reboot()
        {
            attached = false;

            scripts = new List<Tuple<int, string>>();
            cheatEngine = new Engine();
            cheatEngine.loadEngine();
        }

        private List<Tuple<string, string>> ParseAllScripts()
        {
            List<Tuple<string, string>> scriptsCollection = new List<Tuple<string, string>>();
            string xmlPath = "Script.xml";
            if (File.Exists(xmlPath))
            {
                using (XmlReader reader = XmlReader.Create(xmlPath))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "AutoAssembly":
                                    {
                                        string name = reader["Name"];
                                        reader.Read();
                                        scriptsCollection.Add(new Tuple<string, string>(name, reader.Value.Trim()));
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return scriptsCollection;
        }

        private void LoadScripts()
        {
            if (!attached)
            {
                return;
            }

            List<Tuple<string, string>> scriptCollection = ParseAllScripts();
            for (int i = 0; i < scriptCollection.Count; ++i)
            {
                cheatEngine.iAddScript(scriptCollection[i].Item1, scriptCollection[i].Item2);
                cheatEngine.iActivateRecord(i, false);
                scripts.Add(new Tuple<int, string>(i, scriptCollection[i].Item1));
            }
        }

        private int GetScriptId(string scriptName)
        {
            foreach (var T in scripts)
            {
                if (T.Item2 == scriptName)
                    return T.Item1;
            }

            return -1;
        }

        public bool AttachToProcess(string p)
        {
            cheatEngine.iOpenProcess(p.Substring(0, p.IndexOf('-', 0)));
            attached = true;
            LoadScripts();
            return true;
        }

        public List<string> GetProcesses(string processName)
        {
            List<string> processesList = new List<string>();
            string processes;
            cheatEngine.iGetProcessList(out processes);
            foreach (string process in Regex.Split(processes, "\r\n"))
            {
                if (process.Contains(processName))
                    processesList.Add(process);
            }

            return processesList;
        }

        public string GetProcess(string processName)
        {
            List<string> processes = GetProcesses(processName);
            return processes.Count == 0 ? null : processes[0];
        }

        public void EnableScript(string scriptName)
        {
            if (!attached)
            {
                return;
            }

            int i = GetScriptId(scriptName);
            if (i != -1)
            {
                cheatEngine.iActivateRecord(i, true);
            }

        }

        public void DisableScript(string scriptName)
        {
            if (!attached)
            {
                return;
            }

            int i = GetScriptId(scriptName);
            if (i != -1)
            {
                cheatEngine.iActivateRecord(i, false);
            }
        }
    }
}
