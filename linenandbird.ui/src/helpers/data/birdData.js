import axios from "axios";
import config from "../config";

const getAllBirds = () => new Promise((resolve, reject) => {
    axios.get(`${config.baseUrl}/api/birds`)
        .then(response => resolve(response.data))
        .catch(err => reject(err));
});

export {getAllBirds};
