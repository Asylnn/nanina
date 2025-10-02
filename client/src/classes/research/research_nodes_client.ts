import type Modifier from "../modifiers/modifiers";
import ResearchNode from "./research_nodes";


export default class ResearchNodeClient extends ResearchNode
{
    public leftPos = 0
    public topPos = 0
    public requiredBy : string[] = [] 
    public allRequirements : string[] = []
}