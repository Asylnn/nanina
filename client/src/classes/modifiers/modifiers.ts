import OperationType from "./operation_type";
import type StatModifier from "./stat_modifier";

//note : while we could add the abstract keyword here, we can't use it as a type anymore in the props field in vue
export default class Modifier {
    public stat : StatModifier = 0;
    public timeout : number = 60000;
    public amount: number = 1;
    public operationType: OperationType = OperationType.Multiplicative;
}