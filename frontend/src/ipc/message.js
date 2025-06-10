class MessageBus {
    constructor() {
        this.handlers = new Map();
        this.message_id = 0;
        this.pending = new Map();
    }
   
    on = (message_type, handler) => {

        if (!this.handlers.has(message_type)) {
            this.handlers.set(message_type, []);
        }

        this.handlers.get(message_type).push(handler);
    }
   
    send = async (message_type, data) => {

        const request_id = ++this.message_id;
        const response_key = `${message_type}_${request_id}`;
        
        return new Promise((resolve) => {
           
            this.pending.set(response_key, (response) => {
                this.pending.delete(response_key);
                resolve(response);
            });
           
            const message_data = {
                id: request_id,
                type: message_type,
                send: true,
                data: msgpack.encode(data || {})
            };
           
            const serialized = msgpack.encode(message_data);
            const base64 = btoa(String.fromCharCode(...new Uint8Array(serialized)));
            window.external.sendMessage(base64);
        });
    }
   
    handle_message = (raw_message) => {

        try {

            const data = Uint8Array.from(atob(raw_message), c => c.charCodeAt(0));
            const message_data = msgpack.decode(data);
            const decoded_message = msgpack.decode(message_data.data);
            
            const response_key = `${message_data.type}_${message_data.id}`;

            if (this.pending.has(response_key)) {
                this.pending.get(response_key)(decoded_message);
                return;
            }
           
            const handlers = this.handlers.get(message_data.type);

            if (handlers) {
                for (const handler of handlers) {
                    handler(decoded_message, message_data.id);
                }
            }
        } catch (error) {
            console.error('message processing error:', error);
        }
    }
    
    send_response = async (response_id, message_type, data) => {

        const message_data = {
            id: response_id,
            type: message_type,
            send: false,
            data: msgpack.encode(data || {})
        };
       
        const serialized = msgpack.encode(message_data);
        const base64 = btoa(String.fromCharCode(...new Uint8Array(serialized)));
       
        window.external.sendMessage(base64);
    }
}

export const ipc = new MessageBus();