import axios from 'axios'
import type { AssignData, GenerateData } from '../types/forms'

export const generateData = async (inputData: GenerateData) => {
	try {
		var response = await axios.post(
			'https://localhost:7264/assignments/generate-data',
			inputData
		)

		return response.data
	} catch (ex) {
		console.error(ex)
	}
}

export const assignHungarian = async (assignData: AssignData) => {
	try {
		console.log(assignData)

		const response = await axios.post(
			'https://localhost:7264/assignments/hungarian',
			assignData
		)
		return response.data
	} catch (ex) {
		console.error(ex)
	}
}

export const assignAuction = async (assignData: AssignData) => {
	try {
		var response = await axios.post(
			'https://localhost:7264/assignments/auction',
			assignData
		)

		return response.data
	} catch (ex) {
		console.error(ex)
	}
}
