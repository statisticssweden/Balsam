import * as React from 'react';
import TextField from '@mui/material/TextField';
import Autocomplete from '@mui/material/Autocomplete';
import CircularProgress from '@mui/material/CircularProgress';
import { Template } from '../Model/RepositoryTemplate';
import { Box, Chip, Stack } from '@mui/material';

export interface RepositoryTemplateAsyncAutocompleteProperties
{
    getTemplates(): Promise<Array<Template>>,
    label: string,
    onChange?(template: Template | null) : void,
    defaultTemplate?: Template
}

export default function RepositoryTemplateAsyncAutocomplete(props : RepositoryTemplateAsyncAutocompleteProperties) {
  const [open, setOpen] = React.useState(false);
  const [options, setOptions] = React.useState<readonly Template[]>([]);
  const loading = open && options.length === 0;

  function renderTags(tags: Array<string>)
  {
      return tags.map((tag) => {
          return <Chip size='small' label={tag}></Chip>;
      })
  }
  
  React.useEffect(() => {
    let active = true;

    if (!loading) {
      return undefined;
    }

    if (active) {
        props.getTemplates().then(options => {
            setOptions(options)
        });
    }
    
    return () => {
      active = false;
    };
  }, [loading]);

  React.useEffect(() => {
    if (!open) {
      setOptions([]);
    }
  }, [open]);

  return (
    <Autocomplete
      id="options"
      size="small"
      sx={{ width: "100%" }}
      open={open}
      defaultValue={props.defaultTemplate}
      onChange={(_event, value) => {
          props.onChange?.(value);
      }}
      onOpen={() => {
        setOpen(true);
      }}
      onClose={() => {
        setOpen(false);
      }}
      isOptionEqualToValue={(option, value) => option.name === value.name}
      getOptionLabel={(option) => option.name}
      options={options}
      loading={loading}
      renderOption={(props, template) => (
        <Box component="li" sx={{ '& > img': { mr: 2, flexShrink: 0 } }} {...props}>
          {template.name}
          <Stack direction="row" sx={{marginLeft:"10px" }} display="inline-block" spacing="3px" >
              {renderTags(template.tags)}
          </Stack>
        </Box>
      )}
      renderInput={(params) => (
        <TextField
          {...params}
          label={props.label}
          variant='standard'
          InputProps={{
            ...params.InputProps,
            endAdornment: (
              <React.Fragment>
                {loading ? <CircularProgress color="inherit" size={20} /> : null}
                {params.InputProps.endAdornment}
              </React.Fragment>
            ),
          }}
        />
      )}
    />
  );
}
