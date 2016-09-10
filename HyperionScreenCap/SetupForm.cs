﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HyperionScreenCap
{
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();
            loadSettings();
        }

        private void loadSettings()
        {
            try
            {
                Settings.LoadSetttings();
                tbIPHostName.Text = Settings.HyperionServerIp;
                tbProtoPort.Text = Settings.HyperionServerPort.ToString();
                cbMessagePriority.Text = Settings.HyperionMessagePriority.ToString();
                tbMessageDuration.Text = Settings.HyperionMessageDuration.ToString();
                tbCaptureWidth.Text = Settings.HyperionWidth.ToString();
                tbCaptureHeight.Text = Settings.HyperionHeight.ToString();
                tbCaptureInterval.Text = Settings.CaptureInterval.ToString();
                cbMonitorIndex.Text = Settings.MonitorIndex.ToString();
                tbReconnectInterval.Text = Settings.ReconnectInterval.ToString();
                cbNotificationLevel.Text = Settings.NotificationLevel.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occcured during LoadSettings(): {ex.Message}");
            }
        }

        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void saveSettings()
        {
            try
            {
                Settings.HyperionServerIp = tbIPHostName.Text;
                Settings.HyperionServerPort = int.Parse(tbProtoPort.Text);
                Settings.HyperionMessagePriority = int.Parse(cbMessagePriority.Text);
                Settings.HyperionMessageDuration = int.Parse(tbMessageDuration.Text);
                Settings.HyperionWidth = int.Parse(tbCaptureWidth.Text);
                Settings.HyperionHeight = int.Parse(tbCaptureHeight.Text);
                Settings.CaptureInterval = int.Parse(tbCaptureInterval.Text);
                Settings.MonitorIndex = int.Parse(cbMonitorIndex.Text);
                Settings.ReconnectInterval = int.Parse(tbReconnectInterval.Text);
                Settings.NotificationLevel =
                    (Form1.NotificationLevels) Enum.Parse(typeof(Form1.NotificationLevels), cbNotificationLevel.Text);

                Settings.SaveSettings();
                Form1.Init(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occcured during SaveSettings(): {ex.Message}");
            }

            Close();
        }

        private static bool validatorInt(string input, int minValue, int maxValue, bool validateMaxValue)
        {
            bool isValid = false;
            int value;
            bool isInteger = int.TryParse(input, out value);

            if (isInteger)
            {
                //Only check minValue
                if (validateMaxValue == false && value >= minValue)
                {
                    isValid = true;
                }
                //Check both min/max values
                else
                {
                    if (value >= minValue && value <= maxValue)
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        private void tbProtoPort_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 1;
            const int maxValue = 65535;
            if (validatorInt(tbProtoPort.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for port");
            }
        }

        private void cbMessagePriority_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(cbMessagePriority.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for message priority");
            }
        }

        private void cbMonitorIndex_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(cbMonitorIndex.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for monitor index");
            }
        }

        private void tbMessageDuration_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = -1;
            const int maxValue = 0;
            if (validatorInt(tbMessageDuration.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for message duration");
            }
        }

        private void tbCaptureWidth_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(tbCaptureWidth.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for capture width");
            }
        }

        private void tbCaptureHeight_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for capture height");
            }
        }

        private void tbCaptureInterval_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(tbCaptureInterval.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for capture interval");
            }
        }

        private void tbReconnectInterval_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if (validatorInt(tbReconnectInterval.Text, minValue, maxValue, false) == false)
            {
                MessageBox.Show(@"Invalid integer filled for reconnect interval");
            }
        }
    }
}
