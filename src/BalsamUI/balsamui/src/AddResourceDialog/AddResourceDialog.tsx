import { Fragment, useContext, useEffect, useState } from "react";
import { KnowledgeLibrary, Project } from "../../BalsamAPI/BalsamAPI/Model";
import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { useDispatch } from "react-redux";
import AppContext, { AppContextState } from "../configuration/AppContext";
import { postError, postSuccess } from "../Alerts/alertsSlice";
import KnowledgeLibraries from "../KnowledgeLibraries/KnowledgeLibraries";
import { Resource } from "../Model/Resource";
import { DataGrid, GridColDef, GridRowSelectionModel, GridToolbar, GridValueGetterParams } from "@mui/x-data-grid";

export interface AddResourceDialogProperties
{
    project: Project,
    branch: string,
    onClosing: () => void,
    onResourceAdded?: () => void,
    open: boolean,
}

interface RowItem
{
    id: string,
    knowledgeLibrary: KnowledgeLibrary,
    resource: Resource
}

export default function AddResourceDialog(props: AddResourceDialogProperties)
{
    const [busy, setBusy] = useState(false);
    const [okEnabled, setOkEnabled] = useState(false);
    const [knowledgeLibraryResoruces, setKnowledgeLibraryResoruces] = useState<Array<RowItem>>();
    const [selectionModel, setSelectionModel] = useState<GridRowSelectionModel>([]);
    const dispatch = useDispatch();

    const appContext = useContext(AppContext) as AppContextState;

    useEffect(() => {
        
        if (!props.open)
        {
            return;
        }

        KnowledgeLibraries.getAllResources(appContext.balsamApi.knowledgeLibraryApi)
        .then(resources => {
            let rowItems= resources.map((r):RowItem => {
                return { id: `${r.knowledgeLibrary.id}_${r.resource.fileId}`, 
                        knowledgeLibrary: r.knowledgeLibrary,
                        resource: r.resource
                    }
            })
            setKnowledgeLibraryResoruces(rowItems);
            updateOkEnabled(busy);
        })
        
    }, [props.project.id, props.branch, props.open]);

    useEffect(() => {
        updateOkEnabled(busy);

    }, [busy]);

    useEffect(() => {
        updateOkEnabled(busy);

    }, [selectionModel]);

    const updateOkEnabled = (busy: boolean) => 
    {
        let itemsSelected = selectionModel.length > 0;
        setOkEnabled(itemsSelected && !busy)
    }

    const handleCancel = () => {
        props.onClosing();
        resetDialog();
    };

    const handleClose = () => {
        props.onClosing();
        resetDialog();
    };

    const resetDialog = () => {
        setKnowledgeLibraryResoruces([]);
        setSelectionModel([]);
        setBusy(false);
    };

    function onResourcesAdded()
    {
        props.onClosing();
        props.onResourceAdded?.();
        resetDialog();
    }

    const handleAdd = () => {
   
        setBusy(true);
        updateOkEnabled(true);

        if (knowledgeLibraryResoruces && selectionModel.length > 0)
        {
            let selectedId = selectionModel[0];

            let selectedKnowledgeLibraryResource = knowledgeLibraryResoruces.find(r => r.id == selectedId);

            if (selectedKnowledgeLibraryResource && selectedKnowledgeLibraryResource.resource.fileId)
            {
                appContext.balsamApi.projectApi.copyFromKnowleadgeLibrary(
                    props.project.id, 
                    props.branch,
                    selectedKnowledgeLibraryResource!.knowledgeLibrary.id, 
                    selectedKnowledgeLibraryResource!.resource.fileId!)
                .then(() => {
                    dispatch(postSuccess( `Resurs kopierad.`)); //TODO: Language
                    onResourcesAdded();
                    setBusy(false);
                })
                .catch(() => {
                    dispatch(postError( `Det gick inte att kopiera resurs`)); //TODO: Language
                    setBusy(false);
                });
            }
        }
    };

    function renderProgress()
    {
        if(busy)
        {
            return (<CircularProgress size="18px" sx={{marginLeft:"6px"}} />)
        }
        else 
        {
            return "";
        }
    }
    
    function selectionChanged(model: GridRowSelectionModel)
    {
        setSelectionModel(model);

    }

    const progress = renderProgress();

    const columns: GridColDef[] = [
        {
          field: 'knowledgelibraryName',
          headerName: 'Kunskapsbibliotek',
          width: 150,
          editable: false,
          valueGetter: (params: GridValueGetterParams) =>
          `${params.row.knowledgeLibrary.name }`,
        },
        {
          field: 'resourceName',
          headerName: 'Namn',
          flex:1,
          editable: false,
          valueGetter: (params: GridValueGetterParams) =>
          `${params.row.resource.name }`,
        },
      ];

    return (
        <Fragment>
                <Dialog
                    open={props.open}
                    onClose={handleClose}
                    fullWidth={true}
                    disableRestoreFocus 
                >
                    <DialogTitle>Lägg till resurs
                        {progress}
                    </DialogTitle>
                    <DialogContent>
                        <DialogContentText>
                            Välj de resurser som ska läggas till
                        </DialogContentText>
                        <Box
                            noValidate
                            sx={{
                                display:'flex',
                                flexDirection: 'column',
                                m:'auto'
                            }}
                            component='form'>
                            {/* TODO: Language */}
                            
                            <DataGrid
                                rows={knowledgeLibraryResoruces || []}
                                slots={{ toolbar: GridToolbar }}
                                columns={columns}
                                onRowSelectionModelChange={selectionChanged}
                                rowSelectionModel={selectionModel}
                                initialState={{
                                pagination: {
                                    paginationModel: {
                                    pageSize: 5,
                                    },
                                },
                                }}
                                pageSizeOptions={[5]}
                            />

                        </Box>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCancel} >Avbryt</Button>
                        <Button onClick={handleAdd} disabled={!okEnabled}>Lägg till</Button>
                    </DialogActions>
                </Dialog>
            </Fragment>
    )
}