import authService from './api-authorization/AuthorizeService'
import { toast } from 'react-toastify';

export async function fetchPost(address, postObject, onSuccess, onError) {
    const token = await authService.getAccessToken();
    fetch(address, {
        method: 'POST',
        headers: !token ? {} : {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(postObject)
    }).then((response) => {
        if (response.ok) {
            const contentType = response.headers.get('Content-Type');
            if (contentType === "text/plain; charset=utf-8") {
                return response.text();
            } else if (contentType === "application/json; charset=utf-8") {
                return response.json();
            }
            else {
                return true;
            }
        }
        else if (response.status === 401) {
            return authService.signIn();
        }
        else if (!response.ok) {
            throw new Error({
                status: response.status,
                errorMessage: 'Something went wrong'
            });
        }
    }).then((data) => {
        onSuccess(data)
    }).catch((error) => {
        if (onError != undefined) {
            onError(error);
        }
        else {
            console.error(error);
            toast.error(error.errorMessage, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
    });
}