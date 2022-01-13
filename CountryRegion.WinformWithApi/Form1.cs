using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CountryRegion.WinformWithApi.Models;

namespace CountryRegion.WinformWithApi
{
    public partial class Form1 : Form
    {
        HttpClient Client;
        public Form1()
        {
            InitializeComponent();
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:7137/");
            GetData();
        }

        #region Get Data

        private async void GetData()
        {
            var responce = await Client.GetAsync("api/Country/GetAll");
            var responceR = await Client.GetAsync("api/Region/GetAll");
            var data = await responce.Content.ReadAsStringAsync();
            var dataR = await responceR.Content.ReadAsStringAsync();
            CountryGrid.DataSource = JsonConvert.DeserializeObject<IList<Country>>(data);
            RegionGrid.DataSource = JsonConvert.DeserializeObject<IList<Models.Region>>(dataR);

            var country = JsonConvert.DeserializeObject<IList<Country>>(data);
            RAddCountryComBox.Items.Clear();
            RUpdateCountryComBox.Items.Clear();
            RDeleteCountryComBox.Items.Clear();
            foreach (var c in country)
            {
                RAddCountryComBox.Items.Add(c.Name);
                RUpdateCountryComBox.Items.Add(c.Name);
                RDeleteCountryComBox.Items.Add(c.Name);
            }

        }

        #endregion

        #region Add Country
        private async void CAddBtn_Click(object sender, EventArgs e)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("FullName", CAddName.Text),
                new KeyValuePair<string, string>("ShortName", CAddShortName.Text),
                new KeyValuePair<string, string>("Code", CAddCode.Text)
            });

            
            var responce = await Client.PostAsync("api/Country/Add", formContent);
            //responce.EnsureSuccessStatusCode();

            if (responce.IsSuccessStatusCode)
            {
                CAddName.Text = String.Empty;
                CAddShortName.Text = String.Empty;
                CAddCode.Text = String.Empty;
            }
            GetData();
        }

        #endregion

        #region Update Country
        private async void CGetUpdateBtn_Click(object sender, EventArgs e)
        {
            var responce = await Client.GetAsync($"api/Country/GetById/{int.Parse(CIdGetUpdate.Text)}");
            if (responce.IsSuccessStatusCode)
            {
                var data = await responce.Content.ReadAsStringAsync();
                var country = JsonConvert.DeserializeObject<Country>(data);
                CUpdateName.Text = country.Name;
                CUpdateShortName.Text = country.ShortName;
                CUpdateCode.Text = country.Code;
                CRegionsUpdate.Text = "";
                foreach (var region in country.Regions)
                    CRegionsUpdate.Text += region.Name + ", ";
            }
            else
                MessageBox.Show((responce.StatusCode).ToString());
        }

        private async void CUpdateBtn_Click(object sender, EventArgs e)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("FullName", CUpdateName.Text),
                new KeyValuePair<string, string>("ShortName", CUpdateShortName.Text),
                new KeyValuePair<string, string>("Code", CUpdateCode.Text)
            });


            var responce = await Client.PutAsync($"api/Country/Update/id?id={CIdGetUpdate.Text}", formContent);
            //responce.EnsureSuccessStatusCode();
            
            if (responce.IsSuccessStatusCode)
            {
                CIdGetUpdate.Text = String.Empty;
                CUpdateName.Text = String.Empty;
                CUpdateShortName.Text = String.Empty;
                CUpdateCode.Text = String.Empty;
                CRegionsUpdate.Text = String.Empty;
            }
            GetData();

        }

        #endregion

        #region Delete Country
        private async void CGetDeleteBtn_Click(object sender, EventArgs e)
        {
            var responce = await Client.GetAsync($"api/Country/GetById/{int.Parse(CIdGetDelete.Text)}");
            if (responce.IsSuccessStatusCode)
            {
                var data = await responce.Content.ReadAsStringAsync();
                var country = JsonConvert.DeserializeObject<Country>(data);
                CDeleteName.Text = country.Name;
                CDeleteShortname.Text = country.ShortName;
                CDeleteCode.Text = country.Code;
                CDeleteRegions.Text = "";
                foreach (var region in country.Regions)
                    CDeleteRegions.Text += region.Name + ", ";
            }
            else
                MessageBox.Show((responce.StatusCode).ToString());
        }

        private async void CDeleteBtn_Click(object sender, EventArgs e)
        {
            var responce = await Client.DeleteAsync("api/Country/Delete/" + CIdGetDelete.Text);
            if (responce.IsSuccessStatusCode)
            {
                CDeleteName.Text = String.Empty;
                CDeleteShortname.Text = String.Empty;
                CDeleteCode.Text = String.Empty;
                CDeleteRegions.Text = String.Empty;
                CIdGetDelete.Text = String.Empty;
            }
            GetData();
        }

        #endregion


        #region Add Region
        private async void RAddBtn_Click(object sender, EventArgs e)
        {
            var responceC = await Client.GetAsync("api/Country/GetAll");
            var dataC = await responceC.Content.ReadAsStringAsync();

            var country = JsonConvert.DeserializeObject<IList<Country>>(dataC);

            long CountryId = (country.FirstOrDefault(p => p.Name == (RAddCountryComBox.SelectedItem).ToString())).Id;

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", RAddName.Text),
                new KeyValuePair<string, string>("ShortName", RAddShortname.Text),
                new KeyValuePair<string, string>("CountryId", CountryId.ToString())
            });


            var responce = await Client.PostAsync("api/Region/Add", formContent);
            //responce.EnsureSuccessStatusCode();

            if (responce.IsSuccessStatusCode)
            {
                RAddName.Text = String.Empty;
                RAddShortname.Text = String.Empty;
                RAddCountryComBox.Text = String.Empty;
            }
            GetData();
        }

        #endregion

        #region Update Region
        private async void RIdUpdateGet_Click(object sender, EventArgs e)
        {
            var responce = await Client.GetAsync($"api/Region/GetById/{int.Parse(RIdUpdate.Text)}");
            if (responce.IsSuccessStatusCode)
            {
                var data = await responce.Content.ReadAsStringAsync();
                var region = JsonConvert.DeserializeObject<Models.Region>(data);
                RUpdateName.Text = region.Name;
                RUpdateShortname.Text = region.ShortName;
                
                var responceC = await Client.GetAsync("api/Country/GetAll");
                var dataC = await responceC.Content.ReadAsStringAsync();
                var country = JsonConvert.DeserializeObject<IList<Country>>(dataC);

                RUpdateCountryComBox.SelectedItem = country.FirstOrDefault(p => p.Id == region.CountryId).Name;
            }
            else
                MessageBox.Show((responce.StatusCode).ToString());
        }

        private async void RUpdateBtn_Click(object sender, EventArgs e)
        {
            var responceC = await Client.GetAsync("api/Country/GetAll");
            var dataC = await responceC.Content.ReadAsStringAsync();
            var country = JsonConvert.DeserializeObject<IList<Country>>(dataC);
            long CountryId = (country.FirstOrDefault(p => p.Name == (RUpdateCountryComBox.SelectedItem).ToString())).Id;


            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", RUpdateName.Text),
                new KeyValuePair<string, string>("ShortName", RUpdateShortname.Text),
                new KeyValuePair<string, string>("CountryId", CountryId.ToString())
            });


            var responce = await Client.PutAsync($"api/Region/Update/id?id={RIdUpdate.Text}", formContent);
            //responce.EnsureSuccessStatusCode();

            if (responce.IsSuccessStatusCode)
            {
                RUpdateName.Text = String.Empty;
                RUpdateShortname.Text = String.Empty;
                RIdUpdate.Text = String.Empty;
                RUpdateCountryComBox.Text = String.Empty;
            }
            GetData();
        }

        #endregion

        #region Delete Region
        private async void RIdDeleteGet_Click(object sender, EventArgs e)
        {
            var responce = await Client.GetAsync($"api/Region/GetById/{int.Parse(RIdDelete.Text)}");
            if (responce.IsSuccessStatusCode)
            {
                var data = await responce.Content.ReadAsStringAsync();
                var region = JsonConvert.DeserializeObject<Models.Region>(data);
                RDeleteName.Text = region.Name;
                RDeleteShortname.Text = region.ShortName;

                var responceC = await Client.GetAsync("api/Country/GetAll");
                var dataC = await responceC.Content.ReadAsStringAsync();
                var country = JsonConvert.DeserializeObject<IList<Country>>(dataC);

                RDeleteCountryComBox.SelectedItem = country.FirstOrDefault(p => p.Id == region.CountryId).Name;
            }
            else
                MessageBox.Show((responce.StatusCode).ToString());
        }

        private async void RDeleteBtn_Click(object sender, EventArgs e)
        {
            var responce = Client.DeleteAsync($"api/Region/Delete/{RIdDelete.Text}").Result;
            if (responce.IsSuccessStatusCode)
            {
                RDeleteName.Text = String.Empty;
                RDeleteShortname.Text = String.Empty;
                RDeleteCountryComBox.Text = String.Empty;
                RIdDelete.Text = String.Empty;
            }
            GetData();
        }
        #endregion
    }
}
