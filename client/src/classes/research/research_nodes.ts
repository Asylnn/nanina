import type Modifier from "../modifiers/modifiers";


export default class ResearchNode
{
    public tier !: number
    public id !: string
    public requirements !: string[]
    public modifiers !: Modifier[]
    public infinite !: boolean
    public cost !: number
}