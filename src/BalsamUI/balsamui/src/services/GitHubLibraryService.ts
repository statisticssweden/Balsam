import { Library } from "../Model/Library"

const githubApiUrl = "https://api.github.com"


const getBranches = async (user: string, repo: string) : Promise<Array<any>> => 
{
    let promise = fetch(`${githubApiUrl}/repos/${user}/${repo}/branches`);
    let result = await promise;
    let branches = await result.json();

    return branches;
}

const getBrancheSha = async (user: string, repo: string, branchName: string) : Promise<Array<any>> => 
{
    let branches = await getBranches(user, repo);

    let branch = branches.find(b => b.name === branchName);

    if (branch === undefined)
    {
        throw(`Branch '${branchName} fanns inte`); //TODO: Language
    }

    return branch.commit.sha;
}



const getFileTree = async (library: Library) => {
    let branchSha = await getBrancheSha(library.user, library.repo, library.branch);

    let promise = fetch(`${githubApiUrl}/repos/${library.user}/${library.repo}/git/trees/${branchSha}`);
    let result = await promise;
    let fileTree = await result.json();
    return fileTree;
}