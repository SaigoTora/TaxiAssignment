export interface TaxiDriver {
	id: number
	location: Location
	name: string
	surname: string
	age: number
	phoneNumber: string
	car: {
		class: string
		brand: string
		licensePlate: string
		color: string
		seatsCount: number
	}
}

export interface Client {
	id: number
	location: Location
	name: string
	surname: string
	age: number
	phoneNumber: string
}

export interface Location {
	lat: number
	lng: number
}

export interface AssignmentResult {
	executionTimeMs: number
	memoryUsedBytes: number
	assignment: number[]
	totalDistanceMeters: number
}
