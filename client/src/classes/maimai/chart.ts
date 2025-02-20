import type Data from "./data"

export default class Chart
{
    public chartID! : string
    public difficulty! : string
    public level! : string
    public playtype! : string
    public versions! : string[]
    public altTitles! : string[]
    public searchTerms! : string[]
    public title! : string
    public data! : Data
    public levelNum! : number
    public id! : number
    public songID! : number
}