//
//  Author:
//    Terry Massey epiphanygs@gmail.com
//
//  Copyright (c) 2015, Terry Massey 
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using MongoDB.Driver;
using RabbitMQ.Client;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using aXon.Worker.EventArgs;
using aXon.Worker.Interfaces;

namespace aXon
{
    internal class MainClass
    {
        private static MessageQueue<TaskMessage> _TaskQueue;
        private static MessageQueue<TaskLogMessage> _LogQueue;
        private static MessageQueue<TaskProgressMessage> _ProgressQueue;
        private static IConnection _Connection;
        

        public static void Main(string[] args)
        {
            InitConnection();
            _TaskQueue = new MessageQueue<TaskMessage>(true, _Connection);
            _LogQueue = new MessageQueue<TaskLogMessage>(false, _Connection);
            _ProgressQueue = new MessageQueue<TaskProgressMessage>(false, _Connection);
            _TaskQueue.OnReceivedMessage += _TaskQueue_OnReceivedMessage;
            _LogQueue.Publish(new TaskLogMessage
                {
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now,
                    TaskId = Guid.Empty,
                    LogLevel = LogLevel.Info,
                    LogMessage = "Node UP: " + Dns.GetHostName()
                });


            Console.ReadLine();
        }

        private static void InitConnection()
        {
            var factory = new ConnectionFactory {HostName = "192.169.164.138"};
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            _Connection = factory.CreateConnection();
        }

        private static void _TaskQueue_OnReceivedMessage(object sender, TaskMessage args)
        {
            _ProgressQueue.Publish(new TaskProgressMessage
                {
                    CurrentTime = DateTime.Now,
                    PercentComplete = 0,
                    StartTime = DateTime.Now,
                    Status = TaskStatus.Arrived,
                    TaskId = args.TaskId,
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now
                });

            switch (args.ScriptType)
            {
                case TaskScriptType.LoadWorker:

                    bool good = false;
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        IEnumerable<Type> types = from x in assembly.GetTypes()
                                                  where typeof (ITaskWorker).IsAssignableFrom(x)
                                                  select x;
                        foreach (Type type in types)
                        {
                            if (type.FullName.Trim() == args.TaskScript.Trim())
                            {
                                var instance = Activator.CreateInstance(type) as ITaskWorker;
                                if (instance != null)
                                {
                                    instance.Progress += TaskProgress;
                                    instance.ErrorOccured += TaskErrorOccured;
                                    instance.Complete += TaskComplete;
                                    instance.Execute(args.TaskId);
                                    good = true;
                                    _ProgressQueue.Publish(new TaskProgressMessage
                                        {
                                            CurrentTime = DateTime.Now,
                                            PercentComplete = 0,
                                            StartTime = DateTime.Now,
                                            Status = TaskStatus.Starting,
                                            TaskId = args.TaskId,
                                            MessageId = Guid.NewGuid(),
                                            TransmisionDateTime = DateTime.Now
                                        });
                                }
                            }
                        }
                    }
                    if (!good)
                    {
                        _ProgressQueue.Publish(new TaskProgressMessage
                            {
                                CurrentTime = DateTime.Now,
                                PercentComplete = 0,
                                StartTime = DateTime.Now,
                                Status = TaskStatus.Failed,
                                TaskId = args.TaskId,
                                MessageId = Guid.NewGuid(),
                                TransmisionDateTime = DateTime.Now,
                                Details = "Could not find task: " + args.TaskScript
                            });
                    }
                    break;
                case TaskScriptType.CSharp:
                    break;
                case TaskScriptType.Python:
                    break;
                case TaskScriptType.Shell:
                    break;
            }
            _ProgressQueue.ListenForNext();
        }

        private static void TaskComplete(object sender, OnCompletionArgs args)
        {
            _ProgressQueue.Publish(new TaskProgressMessage
                {
                    CurrentTime = DateTime.Now,
                    PercentComplete = 100,
                    StartTime = DateTime.Now,
                    Status = TaskStatus.Complete,
                    TaskId = args.TaskId,
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now
                });
        }

        private static void TaskErrorOccured(object sender, OnErrorArgs args)
        {
            _LogQueue.Publish(new TaskLogMessage
                {
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now,
                    TaskId = args.TaskId,
                    LogLevel = LogLevel.Error,
                    LogMessage = args.Error
                });
        }

        private static void TaskProgress(object sender, OnProgressArgs args)
        {
            _ProgressQueue.Publish(new TaskProgressMessage
                {
                    CurrentTime = args.CurrentTime,
                    PercentComplete = args.PercentComplete,
                    StartTime = args.StartTime,
                    Status = args.Status,
                    TaskId = args.TaskId,
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now
                });
        }
    }
}