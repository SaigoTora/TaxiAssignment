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
		const response = await axios.post(
			'https://localhost:7264/assignments/hungarian',
			assignData
		)
		return response.data
	} catch (ex) {
		console.error(ex)
	}
}

export const assignAuctionFixed = async (assignData: AssignData) => {
	try {
		var response = await axios.post(
			'https://localhost:7264/assignments/auction-fixed',
			assignData
		)

		return response.data
	} catch (ex) {
		console.error(ex)
	}
}

export const assignAuctionScaled = async (assignData: AssignData) => {
	try {
		var response = await axios.post(
			'https://localhost:7264/assignments/auction-scaled',
			assignData
		)

		return response.data
	} catch (ex) {
		console.error(ex)
	}
}
