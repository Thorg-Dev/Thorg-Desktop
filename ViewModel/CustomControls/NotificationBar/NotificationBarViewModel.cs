﻿using GolemUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace GolemUI.ViewModel.CustomControls
{

    public class NotificationBarViewModel : INotifyPropertyChanged, IDisposable
    {
        Timer timer = new Timer();

        public NotificationBarViewModel(INotificationService notificationService)
        {

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 100;
            timer.Enabled = false;

            _items = new ObservableCollection<NotificationBarNotification>();
            _notificationService = notificationService;
            _notificationService.NotificationArrived += _notificationService_NotificationArrived;
        }

        public NotificationBarViewModel()
        {
            _items = new ObservableCollection<NotificationBarNotification>();
            Enumerable.Range(1, 5).ToList().ForEach(x => Items.Add(new NotificationBarNotification(true, NotificationState.Visible, $"title {x}", $"id {x}", $"message {x}", 5000, x * 1000, false)));
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Items.Where(x => x.ShouldAutoHide).ToList().ForEach(x => x.LifeTime += (int)timer.Interval);

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Items.Where(x => x.ShouldAutoHide).ToList().ForEach(x => { if (x.ShouldBeRemoved) Items.RemoveAt(0); });
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        INotificationService? _notificationService;
        /* private string __lastNotification = "hello";
         public string LastNotification
         {
             get => __lastNotification;
             set
             {
                 __lastNotification = value;
                 OnPropertyChanged(nameof(LastNotification));
             }
         }
        */
        public ObservableCollection<NotificationBarNotification> _items;
        public ObservableCollection<NotificationBarNotification> Items => _items;




        private void _notificationService_NotificationArrived(INotificationObject ntf)
        {
            AddOrUpdate(new NotificationBarNotification(ntf.ExpirationTimeInMs > 0, NotificationState.Visible, ntf.Title, ntf.GetId(), ntf.Message, ntf.ExpirationTimeInMs, 0, ntf.Group));
        }
        private bool ElementWithSpecifiedIdAlreatExists(string id)
        {
            return Items.ToList().Exists(x => x.Id == id);
        }
        private void TryUpdateElement(NotificationBarNotification ntf)
        {
            var item = Items.ToList().Find(x => x.Id == ntf.Id);
            if (item != default(NotificationBarNotification))
            {
                item.Title = ntf.Title;
                item.ShouldAutoHide = ntf.ShouldAutoHide;
                item.ExpirationTime = ntf.ExpirationTime;
                item.LifeTime = 0;
            }
        }
        private void AddOrUpdate(NotificationBarNotification ntf)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                // LastNotification = ntf.Title;
                if (ElementWithSpecifiedIdAlreatExists(ntf.Id) && ntf.Group)
                    TryUpdateElement(ntf);
                else
                    Items.Add(ntf);

                timer.Enabled = Items.Count > 0;
            });
        }
        private void OnPropertyChanged(string? propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}
