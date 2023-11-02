import { MuiMarkdown } from 'mui-markdown';
import './MarkdownViewer.css';

export default function MarkdownViewer({ markdown }: {markdown: string}) {
    return (
        <div className='markdown-view'> 
            <MuiMarkdown>{markdown}</MuiMarkdown>
        </div>
    );
}