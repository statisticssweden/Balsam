const configFile = '/config.json'

export interface AppConfiguration
{
    apiurl: string
}

const getConfig = async ():Promise<AppConfiguration> => 
{
    let config = await fetch(configFile)
        .then(async (result) => {
            return await result.json()
            .then((json)=> {
                return json;
            })
            .catch(()=> {
                alert("Det gick inte att läsa in konfigurationsfil."); //TODO: Language 
            })
        }
        ).catch(() => {
            alert("Det gick inte att hämta konfigurationsfil."); //TODO: Language
        });

    return config as AppConfiguration;
}

const config = await getConfig();

export default config;
