# How to Provide Authentication to Access Json Data in Reports

This example demonstrates how to provide a report's JSON data source with authentication parameters at runtime.

You can see two approaches to provide authentication to the specified Web Service Endpoint:

- **Approach 1**  
You can use a connection string to create a <xref:DevExpress.DataAccess.Json.JsonDataSource> object. The connection string can include authentication parameters.  
In this approach, only the JsonDataSource's connection name is serialized to the report's definition. The JsonDataSource's [JsonSource](xref:DevExpress.DataAccess.Json.JsonDataSource.JsonSource) property is not specified.

- **Approach 2**  
You can use the <Xref:DevExpress.DataAccess.Json.UriJsonSource> object to specify authentication parameters. Assign this object to the JsonDataSource's [JsonSource](xref:DevExpress.DataAccess.Json.JsonDataSource.JsonSource) property.   
In this approach, authentication parameters are serialized to the report's definition together with the JsonDataSource's UriJsonSource object.

Refer to the <xref:400660> topic for more instructions.

Refer to the [JSON Data Source](xref:400377) documentation section for more information on how to bind reports to JSON data.