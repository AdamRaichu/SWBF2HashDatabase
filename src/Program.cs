using System.Text;

int Hash(string strToHash, bool lowercase = false)
{
    if (lowercase) strToHash = strToHash.ToLower();
    int hash = 5381;
    for (int i = 0; i < strToHash.Length; i++)
    {
        byte B = (byte)strToHash[i];
        hash = (hash * 33) ^ B;
    }
    return hash;
}
string ToString(int h) => $"0x{h:X8}";

HashSet<int> allHashes = new HashSet<int>();
HashSet<int> unhashed = new HashSet<int>();
HashSet<int> stillHashed = new HashSet<int>();
HashSet<int> flaggedIn = new HashSet<int>();
List<string> duplicates = new List<string>();
List<string> strings = new List<string>(); // making it a list so it can be sorted nicely later :D
List<string> flaggedOut = new List<string>();

// tmp
//DirectoryInfo dir = new DirectoryInfo("E:\\Workspace\\FrostyToolsuite\\FrostyEditor\\bin\\Developer\\Debug\\BlueprintEditor\\NodeMappings\\EveryHash\\Test");

//foreach (FileInfo file in dir.GetFiles())
//{
//    string text = File.ReadAllText(file.FullName);
//    string[] lines = text.Split('\n');
//    foreach (string line in lines)
//    {
//        string l = line.Trim();
//        if (l == string.Empty) continue;
//        if (l.StartsWith("0x"))
//        {
//            allHashes.Add(int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber));
//            stillHashed.Add(int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber));
//            continue;
//        }
//        int h = hash(l);
//        allHashes.Add(h);
//        strings.Add(l);
//        unhashed.Add(h);
//    }
//}

// Read hashes txt
string text = File.ReadAllText("hashes.txt");
string[] lines = text.Split('\n');
foreach (string line in lines)
{
    string l = line.Trim();
    if (l == string.Empty || !l.StartsWith("0x")) continue; // line is empty or literally isnt a hash
    // int hash = int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber);
    allHashes.Add(int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber));
    stillHashed.Add(int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber));
}

// Read flagged_input.txt
text = File.ReadAllText("flagged_input.txt");
lines = text.Split('\n');
foreach (string line in lines)
{
    string l = line.Trim();
    if (l == string.Empty || !l.StartsWith("0x")) continue;
    flaggedIn.Add(int.Parse(l.Substring(2), System.Globalization.NumberStyles.HexNumber));
}


// Read strings txt
text = File.ReadAllText("strings.txt");
lines = text.Split('\n');
foreach (string line in lines)
{
    string l = line.Trim();
    int hash = Hash(l);
    if (l == string.Empty) continue; // empty
    if (!allHashes.Contains(hash)) continue; // doesnt equate to a hash
    if (strings.Contains(l)) continue; // already here
    stillHashed.Remove(hash);
    if (unhashed.Contains(hash)) {
        // duplicates.Add(l + " - " + ToString(hash));
        duplicates.Add(ToString(hash));
    }
    if (flaggedIn.Contains(hash)) {
        flaggedOut.Add(l + " - " + ToString(hash));
    }; // flagged
    unhashed.Add(hash);
    strings.Add(l);
}
strings.Sort();

//StringBuilder sbAllHashes = new StringBuilder();
StringBuilder sbStrings = new StringBuilder();
StringBuilder sbUnhashed = new StringBuilder();
StringBuilder sbStillHashed = new StringBuilder();
StringBuilder sbCompletion = new StringBuilder();
StringBuilder sbDuplicates = new StringBuilder();
StringBuilder sbFlaggedOut = new StringBuilder();

//foreach (var h in allHashes)
//{
//    sbAllHashes.AppendLine(ToString(h));
//}

foreach (var s in strings)
{
    sbStrings.AppendLine(s);
}
foreach (var h in unhashed)
{
    sbUnhashed.AppendLine(ToString(h));
}
foreach (var h in stillHashed)
{
    sbStillHashed.AppendLine(ToString(h));
}
foreach (var d in duplicates)
{
    sbDuplicates.AppendLine(d);
}
foreach (var f in flaggedOut)
{
    sbFlaggedOut.AppendLine(f);
}

sbCompletion.AppendLine($"Total hashes: {allHashes.Count}");
sbCompletion.AppendLine($"Successfully unhashed: {unhashed.Count} ({(((float)unhashed.Count / allHashes.Count) * 100):0.00}%)");
sbCompletion.AppendLine($"Unsolved: {allHashes.Count - unhashed.Count} ({((((float)allHashes.Count - unhashed.Count) / allHashes.Count) * 100):0.00}%)");

//File.WriteAllText("hashes.txt", sbAllHashes.ToString());
File.WriteAllText("strings.txt", sbStrings.ToString());
File.WriteAllText("unhashed.txt", sbUnhashed.ToString());
File.WriteAllText("unsolved.txt", sbStillHashed.ToString());
File.WriteAllText("result.txt", sbCompletion.ToString());
File.WriteAllText("README.md", sbCompletion.ToString()); // same as result but for github
File.WriteAllText("duplicates.txt", sbDuplicates.ToString());
File.WriteAllText("flagged_output.txt", sbFlaggedOut.ToString());