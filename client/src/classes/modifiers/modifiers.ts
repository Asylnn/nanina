import OperationType from "./operation_type";
import type StatModifier from "./stat_modifier";

export default class Modifier {
    public stat : StatModifier = 0;
    public timeout : number = 60000;
    public amount: number = 1;
    public operationType: OperationType = OperationType.Multiplicative;

    /*
        This is for regrouping multiple modifiers with the same id / operation type into a single modifier
    */
    public static compactModifiers(modifiers : Modifier[]) : Modifier[]
    {
        /*This code is not efficient (o(n²)) at all, I hope the load will be fine on the browser*/
        console.log("0", modifiers)

        modifiers.forEach((modifier1, index1) => {
            for(let index2 = index1; index2 < modifiers.length; index2++)
            {
                let modifier2 = modifiers[index2]
                if(index1 < index2 && modifier1.stat == modifier2.stat && modifier1.operationType == modifier2.operationType)
                {
                    modifier1.amount += modifier2.amount
                    modifiers.splice(index2, 1)
                    index2 -= 1
                }
            }
            /*modifiers.forEach((modifier2, index2) => {
                
                
            })*/
        })

        /*modifiers.forEach((modifier1, index1) => {
        });*/

        return modifiers
    }
}