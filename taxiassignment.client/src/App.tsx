import { Box } from '@chakra-ui/react'
import { GoogleMap, useJsApiLoader } from '@react-google-maps/api'
import GenerateDataForm from './components/forms/GenerateDataForm'
import MapMarkers from './components/MapMarkers'
import {
	assignAuction,
	assignHungarian,
	generateData,
} from './services/assignment'
import type { City, GenerateData } from './types/forms'
import { useState } from 'react'
import AssignmentButtons from './components/forms/AssignmentButtons'

const GOOGLE_MAPS_API_KEY = import.meta.env.VITE_GOOGLE_MAPS_API_KEY

export default function App() {
	const [taxiDrivers, setTaxiDrivers] = useState<
		{ lat: number; lng: number }[]
	>([])
	const [clients, setClients] = useState<{ lat: number; lng: number }[]>([])
	const [map, setMap] = useState<google.maps.Map | null>(null)
	const [distances, setDistances] = useState<[][] | null>(null)
	const [initialCenter] = useState({ lat: 50.455, lng: 30.59 })
	const [initialZoom] = useState(11)

	const CITY_CENTERS: Record<City, google.maps.LatLngLiteral> = {
		Kyiv: { lat: 50.455, lng: 30.59 },
		Kharkiv: { lat: 49.9975, lng: 36.255 },
		Lviv: { lat: 49.83, lng: 24.015 },
	}

	const { isLoaded } = useJsApiLoader({
		googleMapsApiKey: GOOGLE_MAPS_API_KEY,
		libraries: ['marker'],
	})

	const onGenerate = async (inputData: GenerateData) => {
		const data = await generateData(inputData)
		if (!data) return

		setTaxiDrivers(
			data.taxiDrivers.map((t: any) => ({
				id: t.id,
				location: {
					lat: t.location.latitude,
					lng: t.location.longitude,
				},
				name: t.name,
				surname: t.surname,
				age: t.age,
				phoneNumber: t.phoneNumber,
				car: {
					class: t.car.class,
					brand: t.car.brand,
					licensePlate: t.car.licensePlate,
					color: t.car.color,
					seatsCount: t.car.seatsCount,
				},
			}))
		)
		setClients(
			data.clients.map((c: any) => ({
				id: c.id,
				location: {
					lat: c.location.latitude,
					lng: c.location.longitude,
				},
				name: c.name,
				surname: c.surname,
				age: c.age,
				phoneNumber: c.phoneNumber,
			}))
		)
		setDistances(data.distances)

		if (map && inputData.city) {
			const center = CITY_CENTERS[inputData.city]
			map.panTo(center)
			map.setZoom(11)
		}
	}

	const onHungarianAssign = async () => {
		if (!distances) return

		const assignResult = await assignHungarian({ distances })
		if (!assignResult) return

		console.log('Hungarian algorithm results:', assignResult)
	}

	const onAuctionAssign = async () => {
		if (!distances) return

		const assignResult = await assignAuction({ distances })
		if (!assignResult) return

		console.log('Auction algorithm results:', assignResult)
	}

	if (!isLoaded) return <div>Loading map...</div>

	return (
		<div className='relative w-full h-screen'>
			<GoogleMap
				mapContainerStyle={{ width: '100%', height: '100%' }}
				center={initialCenter}
				zoom={initialZoom}
				onLoad={mapInstance => setMap(mapInstance)}
			>
				{map && (
					<MapMarkers map={map} taxiDrivers={taxiDrivers} clients={clients} />
				)}
			</GoogleMap>

			<Box
				position='absolute'
				top='16px'
				left='16px'
				zIndex='10'
				bg='white'
				color='black'
				rounded='xl'
				shadow='md'
				paddingBlock='7'
				paddingInline='10'
				width='20rem'
			>
				<GenerateDataForm onGenerate={onGenerate} />
				{taxiDrivers.length > 0 && clients.length > 0 && (
					<AssignmentButtons
						onHungarianAssign={onHungarianAssign}
						onAuctionAssign={onAuctionAssign}
					/>
				)}
			</Box>
		</div>
	)
}
