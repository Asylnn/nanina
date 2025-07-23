export function MillisecondsToHourMinuteSecondFormat(milli : number) : string
{
    var seconds = Math.floor(milli/1000)
    var minutes = Math.floor(seconds/60)
    seconds %= 60
    var hours = Math.floor(minutes/60)
    minutes %= 60
    if(hours > 0)
        return `${hours}h${minutes}m${seconds}s`
    else if(minutes > 0)
        return `${minutes}m${seconds}s`
    return `${seconds}s`
}

export function getRarityStyle(rarity: number)
{
    let style = ""
    let color = "yellow;"
    if(rarity == 0)
        color = "black;"
    else if(rarity == 1)
        color = "blue;"
    else if(rarity == 2)
        color = "green;"
    else if(rarity == 3)
        color = "purple;"
    return style + "border-color:" + color
}