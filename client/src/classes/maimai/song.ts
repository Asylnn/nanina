import type Lang from "./lang"

export default class Song
{
    public id !: number
    public code !: string
    public name !: Lang
    public artist !: Lang
}
