using System.Collections.Generic;
using System.Linq;
using System.IO;
using ptsCogo.Horizontal;

namespace CogoConsoleApp
{
    class Program
    {
        static DirectoryManager projectDir { get; set; } = null;
        static void Main(string[] args)
        {
            projectDir = new DirectoryManager();
            projectDir.CdUp(2).CdDown("CogoTests").CdDown("R2547");
            string testFile = projectDir.GetPathAndAppendFilename("Y15A.csv");

            rm21HorizontalAlignment Y15A = rm21HorizontalAlignment.createFromCsvFile(testFile);
            var outputPointsFile = @"D:\NCSU\201703 Fall 2017\GIS 630 Independent Study\WorkProduct\CSVdata\Y15A_Computed_05m.csv";
            var allPoints = Y15A.getXYZcoordinateList(5.0,csvOutFileName: outputPointsFile);
        }
    }

    internal class DirectoryManager
    {  // From GitHubGist: https://gist.github.com/PaulSchrum/4fb6015d46d79c06b08acb7f1bb00c53
        // If I add other things (like createDir or move, etc. The version here should be updated.
        public string GetPathAndAppendFilename(string filename = null)
        {
            if(filename == null || filename.Length == 0)
                return this.path;
            return this.path + "\\" + filename;
        }

        public string path { get; protected set; }
        public List<string> pathAsList
        {
            get
            {
                return this.path.Split('\\').ToList();
            }
        }

        protected void setPathFromList(List<string> aList)
        {
            this.path = string.Join("\\", aList);
        }

        public DirectoryManager()
        {
            this.path = System.IO.Directory.GetCurrentDirectory();
        }

        public int depth
        {
            get
            {
                return pathAsList.Count - 1;
            }
        }

        public DirectoryManager CdUp(int upSteps)
        {
            if(upSteps > this.depth) throw new IOException("Can't cd up that high.");
            var wd = this.pathAsList.Take(depth - upSteps).ToList();
            this.setPathFromList(wd);
            return this;
        }

        public DirectoryManager CdDown(string directoryName)
        {
            if(!this.SubDirectories.Contains(directoryName))
                throw new DirectoryNotFoundException();
            var tempList = this.pathAsList;
            tempList.Add(directoryName);
            this.setPathFromList(tempList);
            return this;
        }

        public List<string> SubDirectories
        {
            get
            {
                var v = Directory.GetDirectories(this.path)
                    .Select(s => s.Split('\\').Last())
                    .ToList();
                return v;
            }
        }

        public override string ToString()
        {
            return this.path;
        }
    }

}
