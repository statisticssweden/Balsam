import { Box } from "@mui/material";

export interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
    className?: string;
  }
  
export default function CustomTabPanel(props: TabPanelProps) {
    const { children, value, index, className, ...other } = props;
  
    return (
      <div
        role="tabpanel"
        hidden={value !== index}
        id={`simple-tabpanel-${index}`}
        aria-labelledby={`simple-tab-${index}`}
        className={className}
        {...other}
      >
        {value === index && (
          <Box sx={{ p: 3 }}>
            {children}
          </Box>
        )}
      </div>
    );
  }