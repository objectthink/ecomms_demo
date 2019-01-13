using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECOMMS_APP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //add a selection handler to listbox
            _instrumentsListBox.SelectedValueChanged += (s, a) => {
                Console.WriteLine("selected");

                var client = (s as ListBox).SelectedItem;
                var f = new instrumentForm(client as IClient);
                f.Show();
            };

            //create an instance of the enterprise manager
            Manager enterpriseManager = new Manager();
            enterpriseManager.connect("nats://10.52.56.225:4222");
            enterpriseManager.init();

            //observe the enterprise manager
            enterpriseManager.addObserver(new ObserverAdapter((o, h) =>
            {
                //need to wait to notify until after base class has gotton response
                //to role request
                //or have library query first before creating client
                //WIP...

                Thread.Sleep(1000);
                switch (h)
                {
                    case "CLIENTS_CHANGED":
                        var instrumentClients = enterpriseManager.clients.Where((i) => i.role == Role.Instrument);

                        _instrumentsListBox.BeginInvoke(new MethodInvoker(() => {
                            _instrumentsListBox.Items.Clear();
                            foreach (IClient instrumentClient in instrumentClients)
                            {
                                _instrumentsListBox.Items.Add(instrumentClient);
                            }
                        }));

                    break;
                }
            }));
        }
    }
}
