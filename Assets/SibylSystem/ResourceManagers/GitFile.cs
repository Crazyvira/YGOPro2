public class GitFile
{
    public string file_name { get; set; }
    public string file_path { get; set; }
    public int size { get; set; }
    public string encoding { get; set; }
    public string content_sha256 { get; set; }
    public string _ref { get; set; }
    public string blob_id { get; set; }
    public string commit_id { get; set; }
    public string last_commit_id { get; set; }
    public string content { get; set; }
}

public class GitNode
{
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string path { get; set; }
    public string mode { get; set; }
    public bool matches_local { get; set; }
}