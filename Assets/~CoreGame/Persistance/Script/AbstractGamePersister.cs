using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CoreGame
{
    public abstract class AbstractGamePersister<T>
    {
        private string persisterFolderName;
        private string fileExtension;
        private string fileName;

        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        private string folderPath;

        protected AbstractGamePersister(string persisterFolderName, string fileExtension, string fileName)
        {
            this.persisterFolderName = persisterFolderName;
            this.fileExtension = fileExtension;
            this.fileName = fileName;
            folderPath = Path.Combine(Application.persistentDataPath, persisterFolderName);
        }


        public T Load()
        {
            if (Directory.Exists(folderPath))
            {
                var path = this.GetDataPath();
                var directoryFiles = Directory.GetFiles(folderPath, this.fileName + this.fileExtension);
                if (directoryFiles.Length > 0)
                {
                    using (FileStream fileStream = File.Open(path, FileMode.Open))
                    {
                        Debug.Log(MyLog.Format("Loaded : " + path));
                        return (T)binaryFormatter.Deserialize(fileStream);
                    }
                }
            }

            return default(T);
        }

        public bool Save(T dataToSave)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);


            using (FileStream fileStream = File.Open(GetDataPath(), FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, dataToSave);
                Debug.Log(MyLog.Format("Saved : " + GetDataPath()));
                return true;
            }

        }

        public string GetDataPath()
        {
            return Path.Combine(this.folderPath, this.fileName + this.fileExtension);
        }
    }
}
