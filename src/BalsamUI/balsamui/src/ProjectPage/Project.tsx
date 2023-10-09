// import BalsamApi from '../services/BalsamAPIServices';
// import Button from '@mui/material/Button';
// import { Link } from 'react-router-dom';
// import { OpenInNew } from '@mui/icons-material';
// import MenuItem from '@mui/material/MenuItem';
// import Select, { SelectChangeEvent } from '@mui/material/Select';
// import FormControl from '@mui/material/FormControl';
// import MarkdownViewer from '../MarkdownViewer/MarkdownViewer';
// import { useDispatch } from 'react-redux';
// import { useState, useEffect } from 'react';
// import { useParams, useSearchParams } from 'react-router-dom'
// import { postError } from '../Alerts/alertsSlice';
// import { Branch } from '../Model/ApiModels';

// export default function ProjectPage() {
//     const [project, setProject] = useState(null);
//     const [loading, setLoading] = useState(true);
//     const { id } = useParams();
//     const [branches, setBranches] = useState<Branch>();
//     const [defaultBranch, setDefaultBranch] = useState(null);
//     const [selectedBranch, setSelectedBranch] = useState(null);
//     const [readmeMarkdown, setReadmeMarkdown] = useState(null);
//     const { branch } = useSearchParams();

//     const dispatch = useDispatch();

//     useEffect(() => {

//         setLoading(true);
//         const fetchData = async () => {
//             let promise = BalsamApi.getProject();

//             promise.catch(() => {
                
//                 dispatch(postError("Det gick inte att ladda projekt"))
//             })

//             let project = await promise;
//             setProject(project);
//             setLoading(false);


//         };

//         fetchData()
//             .catch(console.error);

//     }, [id]);



//     function renderReadme() {
//         let readmeElement = readmeMarkdown
//             ? <div className="scroll"><MarkdownViewer markdown={readmeMarkdown} /></div>
//             : <p><em>ingen readme</em></p>

//         return (
//             readmeElement
//         );
//     }

//     const selectedBranchChanged = (event: SelectChangeEvent<any>) => {
//         setSelectedBranch(event.target.value);
//         //TODO: Ladda om projekt
//     };

//     function renderBranchSelect() {
//         var selectBranchesElement;

//         if (branches) {
//             selectBranchesElement = (<div className="branchSelect">
//                 <FormControl size="small" variant="standard"   >
//                     <Select
//                         id="branch-select"
//                         value={selectedBranch}
//                         onChange={selectedBranchChanged}
//                     >
//                         {branches.map(branch => (
//                             <MenuItem key={branch.name} value={branch.name}>{branch.name}</MenuItem>
//                         ))}

//                     </Select>
//                 </FormControl>
//             </div>)
//         }

//         return selectBranchesElement;
//     }

//     function renderProject(project)
//     {
//         let readmeElement = renderReadme();
//         let branchSelect = renderBranchSelect();

//         return (
    
//             <div>
//                 <div className="project-header">
//                     <div><h2>{project.name}</h2></div>
//                     <div className="git-box">
//                         <div className="git-box-content">
//                             {branchSelect}
//                             <Button component={Link as any} target="_blank" underline="hover" to={project.url}>Git<OpenInNew fontSize="inherit" /></Button>
//                         </div>
//                     </div>
//                 </div>
//                 {readmeElement}
//                 <h3>Resurser</h3>
//                 {/* <ResourcesSection projectid={project.id} branch={selectedBranch} linkerProjectId={linkerprojectid} linkerProjectBranch={linkerProjectBranch} fetchResources={fetchResources} /> */}
//                 <h3>Bearbetningsmiljöer</h3>
//                 {/* <WorkAreasSection projectid={project.id} /> */}
//             </div>
//             );
//     }

//     let contents = loading
//         ? <p><em>Laddar...</em></p>
//         : renderProject(project);

//     return (
//         <div>
//             {contents}
//         </div>
//     ); 

// }