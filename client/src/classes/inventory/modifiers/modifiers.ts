//note : while we could add the abstract keyword here, we can't use it as a type anymore in the props field in vue
export default class Modifier {
    public id : number = 0;
    public timeout : number = 60000;
    public amount: number = 1;
}