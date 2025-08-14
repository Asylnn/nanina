type typpp = string extends "testing123" ? never : string

export default interface Dictionary<T>{
    [Key: typpp]: T ;
}
