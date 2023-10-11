import BalsamApi, {Project, Branch} from '../services/BalsamAPIServices';
import Button from '@mui/material/Button';
import { Link } from 'react-router-dom';
import { Description, OpenInNew } from '@mui/icons-material';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import MarkdownViewer from '../MarkdownViewer/MarkdownViewer';
import { useDispatch } from 'react-redux';
import { useState, useEffect } from 'react';
import { useParams, useSearchParams } from 'react-router-dom'
import { postError } from '../Alerts/alertsSlice';
import './ProjectPage.css'
import HttpService from '../services/HttpServices';
import { Resource, ResourceType, getResourceType } from '../Model/Model';
import ResourcesSection from '../ResourceSection/ResourcesSection';


export default function ProjectPage() {
    const [project, setProject] = useState<Project>();
    const [loading, setLoading] = useState(true);
    const { id } = useParams<string>();
    const [branches, setBranches] = useState<Array<Branch>>();
    const [defaultBranch, setDefaultBranch] = useState(null);
    const [selectedBranch, setSelectedBranch] = useState<string>();
    const [readmeMarkdown, setReadmeMarkdown] = useState<string>();
    const [resources, setResources] = useState<Array<Resource>>();

    const { branch } = useSearchParams();

    const dispatch = useDispatch();

    const loadFiles = async (projectId: string, branch: string) => {

        if (branch === null || typeof branch === 'undefined') {
            return;
        }

        let promise = BalsamApi.projectApi.getFiles(projectId, branch);
        let response = await promise;
        let files = response.data;

        let readmeFile = files.find((file) => file.path.toLowerCase() === "/readme.md");

        HttpService.getTextFromUrl(readmeFile.contentUrl)
            .then((text) => 
            {   
                setReadmeMarkdown(text);
            })
            .catch( () => {
                setReadmeMarkdown("Fel vid inläsning av README.md"); //TODO: Language
            });

        let resourceFiles = files.filter((file) => {
            return file.path.startsWith('/Resources/') || file.path.toLowerCase() === "/readme.md"
        });

        let resourcesArray = await Promise.all(resourceFiles.map( async (file): Promise<Resource> => {
            let name = file.name;
            name = name.replace(/\.[^/.]+$/, "");
            
            let type = getResourceType(file.name);
            let linkUrl: string = "";
            
            let description = "";
            switch (type) {
                case ResourceType.Md:
                    description = "Markdownfil som går att läsa i gränssnittet"; //TODO: Language
                    break;
                case ResourceType.Url:
                    let content = await HttpService.getTextFromUrl(file.contentUrl);
                    let matches = content.match(/URL\s*=\s*(\S*)/)
                    linkUrl =  matches !== null && matches.length > 0 ? matches[0] : "";
                    description = file.contentUrl;

                    break;
                case ResourceType.Document:
                    description = file.name;
                    break;
                default:
                    description = file.name;
            }


            return { 
                name: name,
                description: description,
                type: type,
                linkUrl: linkUrl,
                contentUrl: file.contentUrl,
                filePath: file.path
                }
        }));

        setResources(resourcesArray);

    };



    useEffect(() => {

        setLoading(true);
        const fetchData = async () => {
            let promise = BalsamApi.projectApi.getProject(id as string);

            promise.catch(() => {
                
                dispatch(postError("Det gick inte att ladda projektet")); //TODO: Language
            })

            let response = await promise;
            setProject(response.data);
            setBranches(response.data.branches);
            setSelectedBranch(response.data.branches?.find((b) => b.isDefault)?.id); 
            setLoading(false);


        };

        fetchData()
            .catch(console.error);

    }, [id]);

    useEffect(() => {
        if (selectedBranch !== undefined)
        {
            loadFiles(id as string, selectedBranch as string);
        }

    }, [selectedBranch]);


    function renderReadme() {
        let readmeElement = readmeMarkdown
            ? <div className="scroll"><MarkdownViewer markdown={readmeMarkdown} /></div>
            : <p><em>ingen readme</em></p>

        return (
            readmeElement
        );
    }

    const selectedBranchChanged = (event: SelectChangeEvent<string>) => {
        setSelectedBranch(event.target.value);
        //TODO: Ladda om projekt
    };

    function renderBranchSelect() {
        var selectBranchesElement;

        if (branches) {
            selectBranchesElement = (<div className="branchSelect">
                <FormControl size="small" variant="standard">
                    <Select
                        id="branch-select"
                        value={selectedBranch}
                        onChange={selectedBranchChanged}
                    >
                        {branches.map(branch => (
                            <MenuItem key={branch.name} value={branch.id}>{branch.name}</MenuItem>
                        ))}

                    </Select>
                </FormControl>
            </div>)
        }

        return selectBranchesElement;
    }

    function renderProject(project: Project)
    {
        let readmeElement = renderReadme();
        let branchSelect = renderBranchSelect();
        
        return (
    
            <div>
                <div className="project-header">
                    <div><h2>{project.name}</h2></div>
                    <div className="git-box">
                        <div className="git-box-content">
                            {branchSelect}
                            {/* <Button component={Link as any} target="_blank" underline="hover" to={project.url}>Git<OpenInNew fontSize="inherit" /></Button> */}
                        </div>
                    </div>
                </div>
                {readmeElement}
                <h3>Resurser</h3>
                <ResourcesSection projectid={project.id!} branch={selectedBranch!} resources={resources} />
                <h3>Bearbetningsmiljöer</h3>
                {/* <WorkAreasSection projectid={project.id} /> */}
            </div>
            );
    }

    let contents = loading
        ? <p><em>Laddar...</em></p>
        : renderProject(project as Project);

    return (
        <div>
            {contents}
        </div>
    ); 

}