import type ServerResponseType from "./server_response_type";

export default interface WebSocketResponse {
    type: ServerResponseType,
    id: string,
    data: any
}