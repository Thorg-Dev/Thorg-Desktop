using GolemUI.Interfaces;
using GolemUI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GolemUI.Src
{
    public class ImageMetadataProvider : IImageMetadata
    {
        private static readonly HttpClient client = new HttpClient();

        public event PropertyChangedEventHandler? PropertyChanged;

        IProcessController _processController;
        IStatusProvider _statusProvider;

        private ProjectData? _projectData;
        private BitmapImage? _image;
        private string? _currentAgreementId;

        public ProjectData ProjectData => _projectData;
        public BitmapImage Image => _image;

        public ImageMetadataProvider (IStatusProvider statusProvider, IProcessController processController)
        {
            _processController = processController;
            _statusProvider = statusProvider;

            _statusProvider.PropertyChanged += _statusProvider_PropertyChanged;
        }

        private void _statusProvider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activities")
            {
                ICollection<ActivityState>? act = _statusProvider.Activities;

                if (act == null)
                {
                    throw new Exception("Activities cannot be null!");
                }

                foreach (ActivityState actState in _statusProvider.Activities ?? new List<ActivityState>())
                {
                    if (actState.ExeUnit == "vm")
                    {
                        if (actState.AgreementId == _currentAgreementId)
                        {
                            return;
                        }
                        _currentAgreementId = actState.AgreementId;
                        UpdateDisplayedImageNameAndIcon(actState);
                        return;
                    }
                }
                _projectData = null;
                _image = null;
                NotifyChanged("Image");
                NotifyChanged("ProjectData");

            }
        }

        private async void UpdateDisplayedImageNameAndIcon(Model.ActivityState gminerState)
        {
            if (gminerState.AgreementId == null) return;

            var agreement = await _processController.GetAgreement(gminerState.AgreementId);
            if (agreement == null) return;

            try
            {
                var hash = agreement.Demand.Properties["golem.srv.comp.task_package"].ToString();
                hash = hash.Split(':')[2];

                _projectData = await GetProjectDataByImage(hash);
                _image = await GetImageVisualRepresentation(hash);
                NotifyChanged("Image");
                NotifyChanged("ProjectData");
            }
            catch (Exception ex)
            {
                return;
            }


        }

        public async Task<ProjectData?> GetProjectDataByImage(string imageHash)
        {
            try
            {
                var projectDataRequest = client.GetAsync("http://63.32.45.128/meta/hash/" + imageHash);
                var projectDataResponse = await projectDataRequest;

                if (projectDataResponse.StatusCode != System.Net.HttpStatusCode.OK) return null;

                var responseBody = await projectDataResponse.Content.ReadAsStringAsync();

                ProjectData weatherForecast = JsonConvert.DeserializeObject<ProjectData>(responseBody);

                return weatherForecast;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BitmapImage> GetImageVisualRepresentation(string imageHash)
        {
            try
            {
                var iconRequest = client.GetAsync("http://63.32.45.128/api/icon/" + imageHash);
                var iconResponse = await iconRequest;

                if (iconResponse.StatusCode != System.Net.HttpStatusCode.OK) return null;

                var iconBytes = await iconResponse.Content.ReadAsByteArrayAsync();

                return LoadImage(iconBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
