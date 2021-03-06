﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using SlimDX.Windows;
using HyperionScreenCap.Model;
using System.IO;
using log4net;
using System.Diagnostics;
using HyperionScreenCap.Helper;

namespace HyperionScreenCap
{
    public partial class SetupForm : Form
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(SetupForm));

        public SetupForm()
        {
            LOG.Info("Instantiating SetupForm");
            InitializeComponent();

            // Automatically set the monitor index
            for ( int i = 0; i < DisplayMonitor.EnumerateMonitors().Length; i++ )
            {
                cbMonitorIndex.Items.Add(i);
            }

            LoadSettings();

            tbHelp.Text = Resources.SetupFormHelp;
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LOG.Info("SetupForm Instantiated");
        }

        private void LoadSettings()
        {
            try
            {
                LOG.Info("Loading settings using SettingsManager");
                SettingsManager.LoadSetttings();
                tbIPHostName.Text = SettingsManager.HyperionServerIp;
                tbProtoPort.Text = SettingsManager.HyperionServerPort.ToString();
                cbMessagePriority.Text = SettingsManager.HyperionMessagePriority.ToString();
                tbMessageDuration.Text = SettingsManager.HyperionMessageDuration.ToString();
                tbCaptureWidth.Text = SettingsManager.HyperionWidth.ToString();
                tbCaptureHeight.Text = SettingsManager.HyperionHeight.ToString();
                tbCaptureInterval.Text = SettingsManager.CaptureInterval.ToString();
                cbMonitorIndex.Text = SettingsManager.MonitorIndex.ToString();
                chkCaptureOnStartup.Checked = SettingsManager.CaptureOnStartup;
                chkPauseUserSwitch.Checked = SettingsManager.PauseOnUserSwitch;
                chkPauseSuspend.Checked = SettingsManager.PauseOnSystemSuspend;
                tbApiPort.Text = SettingsManager.ApiPort.ToString();
                chkApiEnabled.Checked = SettingsManager.ApiEnabled;
                chkApiExcludeTimesEnabled.Checked = SettingsManager.ApiExcludedTimesEnabled;
                tbApiExcludeStart.Text = SettingsManager.ApiExcludeTimeStart.ToString("HH:mm");
                tbApiExcludeEnd.Text = SettingsManager.ApiExcludeTimeEnd.ToString("HH:mm");
                rbcmDx9.Checked = SettingsManager.CaptureMethod == CaptureMethod.DX9;
                rbcmDx11.Checked = SettingsManager.CaptureMethod == CaptureMethod.DX11;
                tbDx11MaxFps.Text = SettingsManager.Dx11MaxFps.ToString();
                tbDx11FrameCaptureTimeout.Text = SettingsManager.Dx11FrameCaptureTimeout.ToString();
                chkCheckUpdate.Checked = SettingsManager.CheckUpdateOnStartup;

                RestoreComboBoxValues();

                cbNotificationLevel.Text = SettingsManager.NotificationLevel.ToString();
                LOG.Info("Finished loading settings using SettingsManager");
            }
            catch ( Exception ex )
            {
                LOG.Error("Failed to load settings into the setup form", ex);
                MessageBox.Show($"Error occcured during LoadSettings(): {ex.Message}");
            }
        }

        private void RestoreComboBoxValues()
        {
            LOG.Info("Resolving combo box values");
            int scalingFactorIndexToSelect = 0;
            foreach ( object obj in cbDx11ImgScalingFactor.Items )
            {
                if ( obj.Equals(SettingsManager.Dx11ImageScalingFactor.ToString()) )
                    break;
                scalingFactorIndexToSelect++;
            }
            cbDx11ImgScalingFactor.SelectedIndex = scalingFactorIndexToSelect;

            cbDx11AdapterIndex.SelectedIndex = SettingsManager.Dx11AdapterIndex;
            cbDx11MonitorIndex.SelectedIndex = SettingsManager.Dx11MonitorIndex;
        }

        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            LOG.Info("Save button clicked");
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                // Check if all settngs are valid
                if ( ValidatorDateTime(tbApiExcludeStart.Text) == false )
                {
                    MessageBox.Show("Error in excluded API start time", "Error in excluded API start time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if ( ValidatorDateTime(tbApiExcludeEnd.Text) == false )
                {
                    MessageBox.Show("Error in excluded API end time", "Error in excluded API end time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                LOG.Info("Saving settings using SettingsManager");
                SettingsManager.HyperionServerIp = tbIPHostName.Text;
                SettingsManager.HyperionServerPort = int.Parse(tbProtoPort.Text);
                SettingsManager.HyperionMessagePriority = int.Parse(cbMessagePriority.Text);
                SettingsManager.HyperionMessageDuration = int.Parse(tbMessageDuration.Text);
                SettingsManager.HyperionWidth = int.Parse(tbCaptureWidth.Text);
                SettingsManager.HyperionHeight = int.Parse(tbCaptureHeight.Text);
                SettingsManager.CaptureInterval = int.Parse(tbCaptureInterval.Text);
                SettingsManager.MonitorIndex = int.Parse(cbMonitorIndex.Text);
                SettingsManager.CaptureOnStartup = chkCaptureOnStartup.Checked;
                SettingsManager.PauseOnUserSwitch = chkPauseUserSwitch.Checked;
                SettingsManager.PauseOnSystemSuspend = chkPauseSuspend.Checked;
                SettingsManager.ApiPort = int.Parse(tbApiPort.Text);
                SettingsManager.ApiEnabled = chkApiEnabled.Checked;
                SettingsManager.ApiExcludedTimesEnabled = chkApiExcludeTimesEnabled.Checked;
                SettingsManager.ApiExcludeTimeStart = DateTime.Parse(tbApiExcludeStart.Text);
                SettingsManager.ApiExcludeTimeEnd = DateTime.Parse(tbApiExcludeEnd.Text);
                SettingsManager.CaptureMethod = rbcmDx9.Checked ? CaptureMethod.DX9 : CaptureMethod.DX11;
                SettingsManager.Dx11MaxFps = int.Parse(tbDx11MaxFps.Text);
                SettingsManager.Dx11FrameCaptureTimeout = int.Parse(tbDx11FrameCaptureTimeout.Text);
                SettingsManager.Dx11ImageScalingFactor = int.Parse(cbDx11ImgScalingFactor.SelectedItem.ToString());
                SettingsManager.Dx11AdapterIndex = cbDx11AdapterIndex.SelectedIndex;
                SettingsManager.Dx11MonitorIndex = cbDx11MonitorIndex.SelectedIndex;
                SettingsManager.CheckUpdateOnStartup = chkCheckUpdate.Checked;

                SettingsManager.NotificationLevel =
                    (NotificationLevel) Enum.Parse(typeof(NotificationLevel), cbNotificationLevel.Text);

                SettingsManager.SaveSettings();
                LOG.Info("Saved settings using SettingsManager");
                MainForm.Init(true);
            }
            catch ( Exception ex )
            {
                LOG.Error("Failed to save settings from the setup form", ex);
                MessageBox.Show($"Error occcured during SaveSettings(): {ex.Message}");
            }

            Close();
        }

        private static bool ValidatorInt(string input, int minValue, int maxValue, bool validateMaxValue)
        {
            bool isValid = false;
            int value;
            bool isInteger = int.TryParse(input, out value);

            if ( isInteger )
            {
                //Only check minValue
                if ( validateMaxValue == false && value >= minValue )
                {
                    isValid = true;
                }
                //Check both min/max values
                else
                {
                    if ( value >= minValue && value <= maxValue )
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        public Boolean ValidatorDateTime(string input)
        {
            DateTime dt;
            Boolean IsValid = false;
            bool isDateTime = DateTime.TryParse(input, out dt);
            if ( isDateTime )
            {
                IsValid = true;
            }

            return IsValid;
        }

        private void tbProtoPort_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 1;
            const int maxValue = 65535;
            if ( ValidatorInt(tbProtoPort.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for port");
                e.Cancel = true;
            }
        }

        private void cbMessagePriority_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(cbMessagePriority.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for message priority");
                e.Cancel = true;
            }
        }

        private void cbMonitorIndex_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(cbMonitorIndex.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for monitor index");
                e.Cancel = true;
            }
        }

        private void tbMessageDuration_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = -1;
            const int maxValue = 0;
            if ( ValidatorInt(tbMessageDuration.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for message duration");
                e.Cancel = true;
            }
        }

        private void tbCaptureWidth_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(tbCaptureWidth.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for capture width");
                e.Cancel = true;
            }
        }

        private void tbCaptureHeight_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for capture height");
                e.Cancel = true;
            }
        }

        private void tbCaptureInterval_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(tbCaptureInterval.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for capture interval");
                e.Cancel = true;
            }
        }

        private void tbApiPort_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 1;
            const int maxValue = 65535;
            if ( ValidatorInt(tbApiPort.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for port");
                e.Cancel = true;
            }
        }

        private void tbExcludeStart_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( ValidatorDateTime(tbApiExcludeStart.Text) == false )
            {
                MessageBox.Show("Error in excluded API start time", "Error in excluded API start time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void tbExcludeEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( ValidatorDateTime(tbApiExcludeEnd.Text) == false )
            {
                MessageBox.Show("Error in excluded API end time", "Error in excluded API end time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void tbDx11FrameCaptureTimeout_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 0;
            const int maxValue = 0;
            if ( ValidatorInt(tbDx11FrameCaptureTimeout.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for DX11 frame capture timeout");
                e.Cancel = true;
            }
        }

        private void tbDx11MaxFps_Validating(object sender, CancelEventArgs e)
        {
            const int minValue = 1;
            const int maxValue = 0;
            if ( ValidatorInt(tbDx11MaxFps.Text, minValue, maxValue, false) == false )
            {
                MessageBox.Show(@"Invalid integer filled for DX11 maximum FPS");
                e.Cancel = true;
            }
        }

        private void lblShowDx11Displays_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LOG.Info("Checking for available DX11 monitors");
            String msg;
            String title;
            MessageBoxIcon icon;
            try
            {
                msg = DX11ScreenCapture.GetAvailableMonitors();
                title = "Available Monitors";
                icon = MessageBoxIcon.Information;
            }
            catch
            {
                msg = "Failed to list monitors";
                title = "Error";
                icon = MessageBoxIcon.Error;
            }
            LOG.Info($"{title}: {msg}");
            MessageBox.Show(msg, title, MessageBoxButtons.OK, icon);
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e)
        {
            UpdateChecker.StartUpdateCheck(false);
        }

        private void btnViewLogs_Click(object sender, EventArgs e)
        {
            Process.Start(MiscUtils.GetLogDirectory());
        }
    }
}
