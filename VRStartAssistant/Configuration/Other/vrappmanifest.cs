namespace VRStartAssistant.Configuration.Other;

public class vrappmanifest {
    public string source { get; set; }
    public List<applications> applications { get; set; }
}

public class applications {
    public string app_key { get; set; }
    public string app_type { get; set; }
    public string launch_type { get; set; }
    public string binary_path_windows { get; set; }
    public bool is_dashboard_overlay { get; set; }
    public langs strings { get; set; }
}

public class langs {
    public strings en_us { get; set; }
}

public class strings {
    public string name { get; set; }
    public string description { get; set; }
}