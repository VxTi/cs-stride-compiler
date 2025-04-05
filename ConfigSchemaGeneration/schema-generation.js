import { z }               from 'zod';
import { zodToJsonSchema } from 'zod-to-json-schema';
import { join }            from 'path';
import { writeFileSync }   from 'fs';

console.log( 'Started generating schema' );

const fileName = 'stride-project-config-json-schema.json';

const configSchema = z.object(
    {
        name:
            z.string()
             .describe( 'The name of the project. Required for executable generation.' ),
        author:
            z.string()
             .optional()
             .describe( 'The name of the author of the project.' ),
        version: z.string().describe( 'The version of the project,in semantic versioning format, e.g.' +
                                      ' v1.0.0(-experimental)' ),
        mainFile:
            z.string()
             .optional()
             .describe( 'Refers to the path of the main file in the project. If omitted, the' +
                        ' compiler will look for a file named "main.sr" in the project root.' ),
        root:
            z.string()
             .optional()
             .describe(
                 'This refers to the path of the project root. If omitted, the compiler will attempt to look for' +
                 ' the main file in the current working directory.' ),
        allowExternalDependencies:
            z.boolean()
             .optional()
             .describe( 'Whether to allow external dependencies from the internet' +
                        ' or not, e.g. importing directly from GitHub. Disabled' +
                        ' by default.' ),
        outputPath:
            z.string()
             .optional()
             .describe( 'Path where the output executable will be generated. Defaults to "./build/<executable>"' ),
        dependencies:
            z.array( z.string() )
             .optional()
             .describe(
                 'A list of external dependencies, either by URL or name. If a name is provided, the compiler will' +
                 ' search for them in "./dependencies/<name>"'
             )
    } );

console.log( `Found ${ Object.keys( configSchema.shape ).length } config definitions` );

const targetPath = join( import.meta.dirname, '..', fileName );
const content    = JSON.stringify( zodToJsonSchema( configSchema, 'stride-config' ), null, 2 );

writeFileSync( targetPath, content, { encoding: 'utf8' } );

console.log( `Wrote JSON schema to ${ targetPath }` );