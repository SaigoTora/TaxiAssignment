import axios from 'axios'
import type { GenerateData } from '../types/forms'

export const generateData = async (inputData: GenerateData) => {
	try {
		var response = await axios.post(
			'https://localhost:7264/assignment/generate-data',
			inputData
		)

		return response.data
	} catch (ex) {
		console.error(ex)
	}
}
