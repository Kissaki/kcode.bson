# KCode.BSON

A library for BSON de-/serialization.

Serialization is WIP.

Deserialize a BSON file into a [System.Text.Json.Nodes.JsonObject](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject?view=net-6.0):

```csharp
var f = File.ReadAllBytes("file.bson");
var jsonObject = BsonSerializer.DeserializeDocument(f);
```
