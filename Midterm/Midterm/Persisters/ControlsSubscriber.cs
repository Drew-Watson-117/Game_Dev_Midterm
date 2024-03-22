using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    public class ControlsSubscriber
    {
        private bool saving = false;
        private bool loading = false;
        private string m_fileName;
        private Controls m_controls;


        public ControlsSubscriber(string fileName)
        {
            m_fileName = fileName;
            m_controls = null;
        }

        public void Load()
        {
            lock (this)
            {
                if (!loading)
                {
                    loading = true;
                    var result = FinalizeLoadAsync();
                    result.Wait();
                }
            }
        }

        private async Task FinalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists(m_fileName))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile(m_fileName, FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Controls));
                                    m_controls = (Controls)mySerializer.ReadObject(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Console.WriteLine("Load failed");
                    }
                }

                loading = false;
            });
        }

        public Controls GetControls()
        {
            if (m_controls == null)
            {
                return null;
            }
            return m_controls;
        }
    }
}
