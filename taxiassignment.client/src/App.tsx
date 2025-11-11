import { Box, IconButton } from '@chakra-ui/react'
import { GoogleMap, useJsApiLoader } from '@react-google-maps/api'
import GenerateDataForm from './components/forms/GenerateDataForm'
import MapMarkers from './components/map/MapMarkers'
import {
	generateData,
	assignHungarian,
	assignAuctionFixed,
	assignAuctionScaled,
} from './services/assignment'
import type { City, GenerateData } from './types/forms'
import { useState } from 'react'
import AssignmentButtons from './components/forms/AssignmentButtons'
import type { AssignmentResult, Client, TaxiDriver } from './types/assignment'
import AssignmentResultCard from './components/AssignmentResultCard'
import MapLegend from './components/map/MapLegend'
import { FiChevronUp, FiChevronDown } from 'react-icons/fi'

const LIBRARIES: ('places' | 'marker' | 'drawing')[] = ['marker']
const GOOGLE_MAPS_API_KEY = import.meta.env.VITE_GOOGLE_MAPS_API_KEY

export default function App() {
	const [map, setMap] = useState<google.maps.Map | null>(null)
	const [initialCenter] = useState({ lat: 50.455, lng: 30.55 })
	const [initialZoom] = useState(11)

	const [taxiDrivers, setTaxiDrivers] = useState<TaxiDriver[]>([])
	const [clients, setClients] = useState<Client[]>([])
	const [distances, setDistances] = useState<[][] | null>(null)
	const [isDataReady, setIsDataReady] = useState(false)

	const [hungarianResult, setHungarianResult] =
		useState<AssignmentResult | null>(null)
	const [auctionFixedResult, setAuctionFixedResult] =
		useState<AssignmentResult | null>(null)
	const [auctionScaledResult, setAuctionScaledResult] =
		useState<AssignmentResult | null>(null)
	const [selectedTab, setSelectedTab] = useState<string | null>(null)
	const [isAssignmentRunning, setIsAssignmentRunning] = useState(false)
	const [epsilonPrecision, setEpsilonPrecision] = useState<number | null>(null)
	const [isCollapsed, setIsCollapsed] = useState(false)

	const CITY_CENTERS: Record<City, google.maps.LatLngLiteral> = {
		Kyiv: { lat: 50.455, lng: 30.55 },
		Kharkiv: { lat: 49.9975, lng: 36.255 },
		Lviv: { lat: 49.83, lng: 24.015 },
	}

	const { isLoaded } = useJsApiLoader({
		googleMapsApiKey: GOOGLE_MAPS_API_KEY,
		libraries: LIBRARIES,
	})

	const onGenerate = async (inputData: GenerateData) => {
		const data = await generateData(inputData)
		if (!data) return

		setTaxiDrivers(
			data.taxiDrivers.map((t: any) => ({
				...t,
				location: { lat: t.location.latitude, lng: t.location.longitude },
			}))
		)
		setClients(
			data.clients.map((c: any) => ({
				...c,
				location: { lat: c.location.latitude, lng: c.location.longitude },
			}))
		)
		setDistances(data.distances)
		setHungarianResult(null)
		setAuctionFixedResult(null)
		setAuctionScaledResult(null)
		setIsDataReady(true)

		if (map && inputData.city) {
			const center = CITY_CENTERS[inputData.city]
			map.panTo(center)
			map.setZoom(11)
		}
	}

	const onInputDataChange = () => {
		setHungarianResult(null)
		setAuctionFixedResult(null)
		setAuctionScaledResult(null)
		setIsDataReady(false)
		setTaxiDrivers([])
		setClients([])
		setSelectedTab(null)
	}

	const onHungarianAssign = async () => {
		if (!distances) return
		const assignResult = await assignHungarian({ distances })
		if (!assignResult) return

		setHungarianResult(assignResult)
		setSelectedTab('hungarian')
	}

	const onAuctionFixedAssign = async () => {
		if (!distances) return
		const assignResult = await assignAuctionFixed({ distances })
		if (!assignResult) return

		setAuctionFixedResult(assignResult)
		setSelectedTab('auction-fixed')
	}

	const onAuctionScaledAssign = async (epsilonPrecision: number) => {
		if (!distances) return
		const assignResult = await assignAuctionScaled({
			distances,
			epsilonPrecision,
		})
		if (!assignResult) return

		setEpsilonPrecision(epsilonPrecision)
		setAuctionScaledResult(assignResult)
		setSelectedTab('auction-scaled')
	}

	if (!isLoaded) return <div>Loading map...</div>

	return (
		<div className='relative w-full h-screen'>
			<GoogleMap
				mapContainerStyle={{ width: '100%', height: '100%' }}
				center={initialCenter}
				zoom={initialZoom}
				options={{ fullscreenControl: false }}
				onLoad={mapInstance => setMap(mapInstance)}
			>
				{map && (
					<MapMarkers
						map={map}
						taxiDrivers={taxiDrivers}
						clients={clients}
						hungarianResult={hungarianResult}
						auctionFixedResult={auctionFixedResult}
						auctionScaledResult={auctionScaledResult}
					/>
				)}
				<MapLegend />
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
				width={isCollapsed ? '7rem' : '22rem'}
			>
				<IconButton
					aria-label='Toggle panel'
					position='absolute'
					className='top-2 right-2'
					onClick={() => setIsCollapsed(prev => !prev)}
				>
					{isCollapsed ? (
						<FiChevronDown size={18} />
					) : (
						<FiChevronUp size={18} />
					)}
				</IconButton>

				{!isCollapsed && (
					<>
						<GenerateDataForm
							onGenerate={onGenerate}
							onChange={onInputDataChange}
							disabled={isAssignmentRunning}
						/>

						{isDataReady && (
							<Box mt='7'>
								<AssignmentButtons
									onHungarianAssign={onHungarianAssign}
									onAuctionFixedAssign={onAuctionFixedAssign}
									onAuctionScaledAssign={onAuctionScaledAssign}
									onLoadingChange={setIsAssignmentRunning}
								/>
							</Box>
						)}

						{(hungarianResult || auctionFixedResult || auctionScaledResult) && (
							<Box mt='6'>
								<AssignmentResultCard
									hungarianResult={hungarianResult}
									auctionFixedResult={auctionFixedResult}
									auctionScaledResult={auctionScaledResult}
									selectedTab={selectedTab}
									setSelectedTab={setSelectedTab}
									epsilonPrecision={epsilonPrecision}
								/>
							</Box>
						)}
					</>
				)}
			</Box>
		</div>
	)
}
