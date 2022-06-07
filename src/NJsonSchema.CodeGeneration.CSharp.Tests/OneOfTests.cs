using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;
using NJsonSchema.Converters;
using Xunit;

namespace NJsonSchema.CodeGeneration.CSharp.Tests
{
    public class OneOfTests
    {
        [Fact]
        public async Task When_Property_with_OneOf_referenced_schema_without_shared_inherited_then_use_objectType()
        {
            //// Arrange
            var json = @"{
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
        ""polymorphism"": {
           ""oneOf"": [
                {
                    ""$ref"": ""#/definitions/typeA""
                },
                {
                    ""$ref"": ""#/definitions/typeB""   
                }
           ]
        }
    },
    ""definitions"": {
        ""typeA"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""propA"": {
                    ""type"": ""string""
                }
            }
        },
        ""typeB"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""propB"": {
                    ""type"": ""string""
                }
            }            
            
        }
    }
}";   

            var schema = await JsonSchema.FromJsonAsync(json);
            var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings { ClassStyle = CSharpClassStyle.Poco });

            //// Act
            var code = generator.GenerateFile();

            //// Assert            
            Assert.Contains("public object Polymorphism { get; set; }", code);
        }

        [Fact]
        public async Task When_Property_with_OneOf_referenced_schema_have_shared_inherited_then_use_it()
        {
            //// Arrange
            var json = @"{
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
        ""polymorphism"": {
           ""oneOf"": [
                {
                    ""$ref"": ""#/definitions/typeA""
                },
                {
                    ""$ref"": ""#/definitions/typeB""   
                }
           ]
        }
    },
    ""definitions"": {
        ""typeBase"" : {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""propMain"": {
                    ""type"": ""string""
                }
            }    
        },
        ""typeA"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeBase""
                }   
            ],
            ""properties"": {
                ""propA"": {
                    ""type"": ""string""
                }
            }
        },
        ""typeB"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeBase""
                }   
            ],
            ""properties"": {
                ""propB"": {
                    ""type"": ""string""
                }
            }            
            
        }
    }
}";

            var schema = await JsonSchema.FromJsonAsync(json);
            var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings { ClassStyle = CSharpClassStyle.Poco });

            //// Act
            var code = generator.GenerateFile();

            //// Assert            
            Assert.Contains("public TypeBase Polymorphism { get; set; }", code);
        }

        [Fact]
        public async Task When_Property_with_OneOf_referenced_schemas_have_complex_inheritance_uses_closest_possible_base()
        {
            //// Arrange
            var json = @"{
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
        ""polymorphism"": {
           ""oneOf"": [
                {
                    ""$ref"": ""#/definitions/typeOneA""
                },
                {
                    ""$ref"": ""#/definitions/typeOneB""   
                },
                {
                    ""$ref"": ""#/definitions/typeTwoB""
                },
                {
                    ""$ref"": ""#/definitions/typeTwoC""   
                }
           ]
        }
    },
    ""definitions"": {
        ""typeOneA"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeShared""
                }   
            ],
            ""properties"": {
                ""one"": {
                    ""type"": ""string""
                }
            }
        },
        ""typeOneB"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeOneA""
                }   
            ],
            ""properties"": {
                ""two"": {
                    ""type"": ""string""
                }
            }            
            
        },
        ""typeTwoA"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeShared""
                }   
            ],
            ""properties"": {
                ""three"": {
                    ""type"": ""string""
                }
            }            
            
        },
        ""typeTwoB"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeTwoA""
                }   
            ],
            ""properties"": {
                ""four"": {
                    ""type"": ""string""
                }
            }              
        },
        ""typeTwoC"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""allOf"": [
                {
                    ""$ref"": ""#/definitions/typeTwoB""
                }   
            ],
            ""properties"": {
                ""four"": {
                    ""type"": ""string""
                }
            }              
        },

        ""typeShared"" : {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""shared"": {
                    ""type"": ""string""
                }
            }    
        },

    }
}";

            var schema = await JsonSchema.FromJsonAsync(json);
            var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings { ClassStyle = CSharpClassStyle.Poco });

            //// Act
            var code = generator.GenerateFile();

            //// Assert            
            Assert.Contains("public TypeShared Polymorphism { get; set; }", code);
        }

    

    }
}
