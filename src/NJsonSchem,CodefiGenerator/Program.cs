using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using System.Text.RegularExpressions;

namespace NJsonSchem_CodefiGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {            
            var schema = await JsonSchema.FromFileAsync(@"C:/Temp/arquivosgerador/ModeloTeste.json");

            var errors = schema.Validate(schema.ToJson(), SchemaType.JsonSchema);

            foreach (var error in errors)
                Console.WriteLine(error.Path + ": " + error.Kind);

            //var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings()
            //{
            //    Namespace = "Meuprojeto.TesteGerador",
            //    GenerateDataAnnotations = false
            //});

            var generator = new MyCSharpGenerator(schema, new CSharpGeneratorSettings()
            {
                Namespace = "Meuprojeto.TesteGerador",
                //GenerateDataAnnotations = false, //[System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
                ExcludedTypeNames = new string[]{ }, //tipos a excluir
                //GenerateDefaultValues = false,
                //TemplateDirectory = 
            });

            var codeFile = generator.GenerateFile();
            File.WriteAllText("C://Temp//arquivosgerador//ModeloTeste.cs", codeFile);

            //TODO: gerar demais arquivos
        }
    }

    public class MyCSharpGenerator : CSharpGenerator
    {
        public MyCSharpGenerator(object rootObject, CSharpGeneratorSettings settings)
            : base(rootObject, settings, new CSharpTypeResolver(settings))
        {
        }

        public override IEnumerable<CodeArtifact> GenerateTypes()
        {
            var schema = (JsonSchema)RootObject;

            return GenerateTypes(schema, schema.Title != null && Regex.IsMatch(schema.Title, "^[a-zA-Z0-9_]*$") ? schema.Title : null);
        }

        public override string GenerateFile()
        {
            var artifacts = GenerateTypes();
            return GenerateFile(artifacts);
        }

        public void GenerateFilesABP(string path)
        {
            var artifacts = GenerateTypes();
            var files = GenerateFile(artifacts);

            File.WriteAllText(path, files);
        }
    }
}