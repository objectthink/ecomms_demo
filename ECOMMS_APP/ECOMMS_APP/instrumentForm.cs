using ECOMMS_Client;
using ECOMMS_Entity;
using MercuryShim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf;

namespace ECOMMS_APP
{
    public partial class instrumentForm : Form
    {
        IClient _client;
        private object client;

        public instrumentForm(IClient client)
        {
            InitializeComponent();

            _client = client;

            //set the titlebar to the name of the client we are watching
            Text = client.name;

            //listen for run state changes
            client.addObserver(new ObserverAdapterEx((anobject, hint, data) =>
            {
                var bytes = (byte[])data;

                var runstate = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                switch (hint)
                {
                    case "datarecord":
                        var wireRecord = MercuryWireRecord.Parser.ParseFrom(bytes);
                        Console.WriteLine(wireRecord.Tag);
                        break;
                    case "RUNSTATE_CHANGED":
                        switch(runstate)
                        {
                            case "Idle":
                                BackColor = Color.Green;
                                break;
                            default:
                                BackColor = Color.Red;
                                break;

                        }
                        break;
                }

            }));

        }
    }
}
