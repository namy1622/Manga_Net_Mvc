using System.Text.Json.Serialization;
using Manga.Home.Models;

public class Chapter_Images{
    [JsonPropertyName("image_file")]
    public string Image_File {set; get;}
}

public class Read_Items{

    [JsonPropertyName("chapter_path")]
    public  string Chapter_Path {set; get;}

    [JsonPropertyName("chapter_image")]
    public  List<Chapter_Images> Chapter_Image{set; get;}
}

public class Data_Read{
    [JsonPropertyName("item")]
    public Read_Items Item{set;get;}
}

public class ApiResponse_Reads{
    [JsonPropertyName("data")]
    public Data_Read data_Read{set; get;}
}