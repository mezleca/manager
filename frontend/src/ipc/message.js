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
    
    send = async (message_type, data, will_send) => {
        const message_data = {
            id: this.message_id++,
            type: message_type,
            send: will_send || false,
            data: msgpack.encode(data)
        };
        
        const serialized = msgpack.encode(message_data);
        const base64 = btoa(String.fromCharCode(...new Uint8Array(serialized)));
        
        window.external.sendMessage(base64);
    }
    
    sendAndWait = async (message_type, data, timeout = 5000) => {

        return new Promise((resolve, reject) => {

            const timer = setTimeout(() => {
                this.pending.delete(message_type);
                reject(new Error("timeout"));
            }, timeout);
            
            this.pending.set(`${message_type}_${this.message_id}`, (response) => {
                clearTimeout(timer);
                resolve(response);
            });
            
            this.send(message_type, data, true);
        });
    }
    
    handleMessage = (rawMessage) => {

        try {

            const data = Uint8Array.from(atob(rawMessage), c => c.charCodeAt(0));
            const message_data = msgpack.decode(data);

            const message = msgpack.decode(message_data.data);
            const response_type = `${message_data.type}_${message_data.id}`;

            if (this.pending.has(response_type)) {
                
                // execute the handler callback
                this.pending.get(response_type)(message);

                // reset message id if needed
                if (this.pending.size === 0) {
                    this.message_id = 0;
                }

                this.pending.delete(response_type);
                return;
            }
            
            const handlers = this.handlers.get(response_type);

            if (handlers) {
                for (let handler of handlers) {
                    handler(message);
                }
            }

        } catch (error) {
            console.error('processing error', error);
        }
    }
}

export const Ipc = new MessageBus();