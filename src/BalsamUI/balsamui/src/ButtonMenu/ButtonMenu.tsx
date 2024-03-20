import * as React from 'react';
import Box from '@mui/material/Box';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import IconButton from '@mui/material/IconButton';
import Settings from '@mui/icons-material/Settings';
import { PropsWithChildren } from 'react';

export interface ButtonMenuOption{
    buttonContent?: any;
    onClick?: (itemKey: any) => void;
    itemKey: any;
}

export interface ButonMenuProperties{
    options: ButtonMenuOption[],
    defaultButtonIndex?: number,
    className?: string,
}

export default function ButtonMenu(props: PropsWithChildren<ButonMenuProperties>) {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleMenuItemClick = (
    index: number,
  ) => {

    let option = props.options[index];
    if(option.onClick)
    {
        option.onClick!(option.itemKey);
    }

    setAnchorEl(null);
  };

  return (
    <React.Fragment>
      <Box sx={{ display: 'flex', alignItems: 'center', textAlign: 'center' }}>
        
          <IconButton
            onClick={handleClick}
            size="small"
            sx={{ ml: 2, marginLeft:"0px" }}
            aria-controls={open ? 'account-menu' : undefined}
            aria-haspopup="true"
            aria-expanded={open ? 'true' : undefined}
          >
            <Settings fontSize="small" ></Settings>
          </IconButton>
        
      </Box>
      <Menu
        anchorEl={anchorEl}
        id="account-menu"
        open={open}
        onClose={handleClose}
        onClick={handleClose}
        PaperProps={{
          elevation: 0,
          sx: {
            overflow: 'visible',
            filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.32))',
            mt: 1.5,
            '& .MuiAvatar-root': {
              width: 32,
              height: 32,
              ml: -0.5,
              mr: 1,
            },
            '&::before': {
              content: '""',
              display: 'block',
              position: 'absolute',
              top: 0,
              right: 14,
              width: 10,
              height: 10,
              bgcolor: 'background.paper',
              transform: 'translateY(-50%) rotate(45deg)',
              zIndex: 0,
            },
          },
        }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        {props.options.map((option, index) => (
                    <MenuItem
                      key={option.itemKey}
                      onClick={() => handleMenuItemClick(index)}
                    >
                      {option.buttonContent}
                    </MenuItem>
                  ))}
      </Menu>
    </React.Fragment>
  );
}
