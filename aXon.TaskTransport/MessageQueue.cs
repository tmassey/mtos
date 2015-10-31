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
using RabbitMQ.Client;
using System.Timers;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client.Events;
using aXon.TaskTransport.Interfaces;
using aXon.TaskTransport.Delegates;

namespace aXon.TaskTransport
{
	public class MessageQueue<T> : IMessageQueue<T>, IDisposable
	{
		private IConnection _connection;
		private IModel channel;
		private Timer _t = new Timer (1000);
		private string _channelName;
	    private bool _getNext = true;
		/// <summary>
		/// 
		/// </summary>
		public MessageQueue (bool AllowReceive, IConnection connection)
		{

			_connection = connection;
			channel = _connection.CreateModel ();
			_channelName = typeof(T).Name;
			channel.QueueDeclare (_channelName, false, false, false, null);
            channel.BasicQos(0, 1, false);
			if (AllowReceive) {
				_t.Elapsed += _t_Elapsed;
				_t.Enabled = true;
				_t.Start ();
			} else {
				_t.Enabled = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool Publish (T message)
		{
			try {
				var sm = JsonConvert.SerializeObject (message);
				var body = Encoding.UTF8.GetBytes (sm);

				channel.BasicPublish ("", _channelName, null, body);
				return true;
			} catch (Exception) {
				return false;

			}
		}

	    public void ListenForNext()
	    {
	        _getNext = true;
	    }

	    private void _t_Elapsed (object sender, ElapsedEventArgs e)
		{
			_t.Stop ();
            var consumer = new EventingBasicConsumer(channel);
			//channel.BasicConsume (_channelName, false, consumer);

            //while (true) {
            //    if (_getNext)
            //    {
                    
			        consumer.Received += (model, ea) =>
			            {
                            _getNext = false;
                            var body = ea.Body;
                            var s = Encoding.UTF8.GetString(body);
                            T obj = JsonConvert.DeserializeObject<T>(s);
                            InvokeReceivedMessage(obj);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
			            };
                    channel.BasicConsume(_channelName, false, consumer);
            //    }
            //}
			//t.Start();
		}

		/// <summary>
		/// </summary>
		public event MessageReceived<T> OnReceivedMessage;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected virtual void InvokeReceivedMessage (T args)
		{
			var handler = OnReceivedMessage;
			if (handler != null)
				handler (this, args);
		}

		public void Dispose ()
		{
			channel.Close (200, "Goodbye");
			_connection.Close ();
		}
	}
}
