using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SteamAccountHistory
{

  public class Extended
  {

    [JsonProperty("onpurchasegrantguestpasspackage")]
    public string Onpurchasegrantguestpasspackage { get; set; }

    [JsonProperty("onpurchasegrantguestpasspackage1")]
    public string Onpurchasegrantguestpasspackage1 { get; set; }

    [JsonProperty("onpurchasegrantguestpasspackage2")]
    public string Onpurchasegrantguestpasspackage2 { get; set; }

    [JsonProperty("onpurchasegrantguestpasspackage3")]
    public string Onpurchasegrantguestpasspackage3 { get; set; }
  }

  public class IdsConverter<T> : JsonConverter<T> where T : List<ulong>, new()
  {
    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
      var converted = new T();
      var token = JToken.Load(reader);
      var childValues = token.Children().Values().Select(t => t.Value<string>());
      converted.AddRange(childValues.Select(ulong.Parse));
      return converted;
    }
  }

  [JsonConverter(typeof(IdsConverter<Appids>))]
  public class Appids : List<ulong>
  {
  }

  [JsonConverter(typeof(IdsConverter<Depotids>))]
  public class Depotids : List<ulong>
  {

  }

  [JsonConverter(typeof(IdsConverter<Appitems>))]
  public class Appitems : List<ulong>
  {
  }

  public class GeneratedPackage
  {

    [JsonProperty("packageid")]
    public ulong PackageId { get; set; }

    [JsonProperty("billingtype")]
    public string BillingType { get; set; }

    [JsonProperty("licensetype")]
    public string LicenseType { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("extended")]
    public Extended Extended { get; set; }

    [JsonProperty("appids")]
    public Appids Appids { get; set; }

    [JsonProperty("depotids")]
    public Depotids Depotids { get; set; }

    [JsonProperty("appitems")]
    public Appitems AppItems { get; set; }
  }
}
