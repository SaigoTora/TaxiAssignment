import { useEffect, useRef } from 'react'
import type {
	AssignmentResult,
	Client,
	TaxiDriver,
	Location,
} from '../../types/assignment'

interface MapMarkersProps {
	map: google.maps.Map | null
	taxiDrivers: TaxiDriver[]
	clients: Client[]
	hungarianResult: AssignmentResult | null
	auctionFixedResult: AssignmentResult | null
	auctionScaledResult: AssignmentResult | null
}

export interface AssignmentLineStyle {
	color: string
	strokeWeight: number
	zIndex: number
}

export const HungarianStyle: AssignmentLineStyle = {
	color: 'lime',
	strokeWeight: 4,
	zIndex: 1,
}

export const AuctionFixedStyle: AssignmentLineStyle = {
	color: 'blue',
	strokeWeight: 2,
	zIndex: 2,
}

export const AuctionScaledStyle: AssignmentLineStyle = {
	color: 'red',
	strokeWeight: 6,
	zIndex: 0,
}

export default function MapMarkers({
	map,
	taxiDrivers,
	clients,
	hungarianResult,
	auctionFixedResult,
	auctionScaledResult,
}: MapMarkersProps) {
	const infoWindowsRef = useRef<google.maps.InfoWindow[]>([])
	const markersRef = useRef<google.maps.Marker[]>([])
	const polylinesRef = useRef<google.maps.Polyline[]>([])
	const lastClickedRef = useRef<number | null>(null)

	useEffect(() => {
		if (!map) return

		lastClickedRef.current = null
		// Clearing the previous state
		clearMapElements()

		const allInfoWindows: google.maps.InfoWindow[] = []
		const taxiMarkers: google.maps.Marker[] = []
		const clientMarkers: google.maps.Marker[] = []
		const hasAnimation =
			taxiDrivers.length + clients.length <= 400 &&
			!hungarianResult &&
			!auctionFixedResult &&
			!auctionScaledResult

		// Creating taxi driver markers
		taxiDrivers.forEach(t => {
			const marker = new google.maps.Marker({
				position: t.location,
				map,
				title: `Taxi: ${t.name} ${t.surname}`,
				icon: {
					url: 'https://img.icons8.com/?size=100&id=j2SfqDojO0h4&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation: hasAnimation ? google.maps.Animation.DROP : undefined,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `
				<div class="bg-white pt-4 pr-2 pb-1 pl-2 rounded-xl shadow-md max-w-xs font-sans text-gray-800">
					<div class="font-bold text-center text-base mb-1 cursor-text">
						${t.name} ${t.surname}, ${t.age}
					</div>
					<div class="text-gray-800 mb-3 cursor-text">
						${t.phoneNumber}
					</div>


					<div class="bg-gray-100 rounded-lg text-sm">
						<div class="flex items-center justify-between mb-1.5">
							<span class="font-semibold mr-2 cursor-text">${t.car.brand}</span>
							<span class="text-xs px-2 py-0.5 rounded-full bg-yellow-100 text-yellow-800 font-medium select-none">
								${t.car.class}
							</span>
						</div>

						<div class="flex items-center justify-between mb-1.5">
							<span class="flex items-center gap-1 mr-2">
								<span class="inline-block w-3 h-3 rounded-full border-2" style="background-color: ${t.car.color.toLowerCase()};"></span>
								<span class="cursor-text">${t.car.color}</span>
							</span>

							<span class="flex items-center font-mono text-xs bg-white border border-gray-300 rounded-md px-1.5 py-0.5 shadow-sm select-none">
								<span class="text-blue-600 mr-1 text-[10px]">
									🇺🇦
								</span>
								<span class="tracking-wider font-semibold">
									${t.car.licensePlate.replace(
										/^(\w{2})(\d{4})(\w{2})$/,
										'$1<span class="mx-[1px] inline-block"></span>$2<span class="mx-[1px] inline-block"></span>$3'
									)}
								</span>
							</span>
						</div>


						<div class="text-xs text-gray-500 text-right cursor-text">
							${t.car.seatsCount} seats
						</div>
					</div>
				</div>`,
			})

			infoWindow.addListener('domready', hideDefaultGoogleInfoUI)

			marker.addListener('click', () => {
				if (lastClickedRef.current === t.id) return
				lastClickedRef.current = t.id

				allInfoWindows.forEach(iw => iw.close())
				infoWindow.open(map, marker)
				drawAssignmentLines(
					map,
					t,
					clients,
					hungarianResult,
					auctionFixedResult,
					auctionScaledResult
				)
			})

			allInfoWindows.push(infoWindow)
			taxiMarkers.push(marker)
		})

		// Creating client markers
		clients.forEach(c => {
			const marker = new google.maps.Marker({
				position: c.location,
				map,
				title: `Client: ${c.name} ${c.surname}`,
				icon: {
					url: 'https://img.icons8.com/?size=100&id=9q3GMpxNIMjC&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation: hasAnimation ? google.maps.Animation.DROP : undefined,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `
				<div class="bg-white pt-4 pr-2 pb-1 pl-2 rounded-xl shadow-md max-w-xs font-sans text-gray-800">
					<div class="font-bold text-center text-base mb-1 cursor-text">
						${c.name} ${c.surname}, ${c.age}
					</div>
					<div class="text-gray-800 mb-1 cursor-text">
						${c.phoneNumber}
					</div>
				</div>`,
			})

			infoWindow.addListener('domready', hideDefaultGoogleInfoUI)

			marker.addListener('click', () => {
				if (lastClickedRef.current === c.id) return
				lastClickedRef.current = c.id

				allInfoWindows.forEach(iw => iw.close())
				infoWindow.open(map, marker)
				drawAssignmentLinesForClient(
					map,
					c,
					taxiDrivers,
					hungarianResult,
					auctionFixedResult,
					auctionScaledResult
				)
			})

			allInfoWindows.push(infoWindow)
			clientMarkers.push(marker)
		})

		// Closing windows and clearing lines when clicking on the map
		const mapClickListener = map.addListener('click', () => {
			lastClickedRef.current = null

			allInfoWindows.forEach(iw => iw.close())
			clearPolylines()
		})

		// Save links to clean them up later
		infoWindowsRef.current = allInfoWindows
		markersRef.current = [...taxiMarkers, ...clientMarkers]

		return () => {
			mapClickListener.remove()
			clearMapElements()
		}
	}, [
		map,
		taxiDrivers,
		clients,
		hungarianResult,
		auctionFixedResult,
		auctionScaledResult,
	])

	function clearMapElements() {
		infoWindowsRef.current.forEach(iw => iw.close())
		markersRef.current.forEach(m => m.setMap(null))
		clearPolylines()
	}

	function clearPolylines() {
		polylinesRef.current.forEach(p => p.setMap(null))
		polylinesRef.current = []
	}

	function hideDefaultGoogleInfoUI() {
		const closeBtn = document.querySelector<HTMLButtonElement>(
			'.gm-ui-hover-effect'
		)
		if (closeBtn) closeBtn.style.display = 'none'
		const header = document.querySelector<HTMLDivElement>('.gm-style-iw-ch')
		if (header) header.style.display = 'none'
	}

	// Draws lines for one taxi driver
	function drawAssignmentLines(
		map: google.maps.Map,
		taxi: TaxiDriver,
		clients: Client[],
		hungarianResult: AssignmentResult | null,
		auctionFixedResult: AssignmentResult | null,
		auctionScaledResult: AssignmentResult | null
	) {
		clearPolylines()

		const taxiIndex = taxiDrivers.findIndex(t => t.id === taxi.id)
		if (taxiIndex === -1) return

		if (hungarianResult?.assignment[taxiIndex] !== undefined) {
			const clientIndex = hungarianResult.assignment[taxiIndex]
			if (clientIndex !== -1 && clients[clientIndex]) {
				drawLine(
					map,
					taxi.location,
					clients[clientIndex].location,
					HungarianStyle
				)
			}
		}

		if (auctionFixedResult?.assignment[taxiIndex] !== undefined) {
			const clientIndex = auctionFixedResult.assignment[taxiIndex]
			if (clientIndex !== -1 && clients[clientIndex]) {
				drawLine(
					map,
					taxi.location,
					clients[clientIndex].location,
					AuctionFixedStyle
				)
			}
		}

		if (auctionScaledResult?.assignment[taxiIndex] !== undefined) {
			const clientIndex = auctionScaledResult.assignment[taxiIndex]
			if (clientIndex !== -1 && clients[clientIndex]) {
				drawLine(
					map,
					taxi.location,
					clients[clientIndex].location,
					AuctionScaledStyle
				)
			}
		}
	}

	// Draws lines for the client (all taxi drivers to whom he is assigned)
	function drawAssignmentLinesForClient(
		map: google.maps.Map,
		client: Client,
		taxiDrivers: TaxiDriver[],
		hungarianResult: AssignmentResult | null,
		auctionFixedResult: AssignmentResult | null,
		auctionScaledResult: AssignmentResult | null
	) {
		clearPolylines()

		const clientIndex = clients.findIndex(c => c.id === client.id)
		if (clientIndex === -1) return

		// For Hungarian
		if (hungarianResult) {
			hungarianResult.assignment.forEach((cIndex, tIndex) => {
				if (cIndex === clientIndex) {
					drawLine(
						map,
						taxiDrivers[tIndex].location,
						client.location,
						HungarianStyle
					)
				}
			})
		}

		// For auction fixed
		if (auctionFixedResult) {
			auctionFixedResult.assignment.forEach((cIndex, tIndex) => {
				if (cIndex === clientIndex) {
					drawLine(
						map,
						taxiDrivers[tIndex].location,
						client.location,
						AuctionFixedStyle
					)
				}
			})
		}

		// For auction scaled
		if (auctionScaledResult) {
			auctionScaledResult.assignment.forEach((cIndex, tIndex) => {
				if (cIndex === clientIndex) {
					drawLine(
						map,
						taxiDrivers[tIndex].location,
						client.location,
						AuctionScaledStyle
					)
				}
			})
		}
	}

	function drawLine(
		map: google.maps.Map,
		start: Location,
		end: Location,
		style: AssignmentLineStyle
	) {
		const line = new google.maps.Polyline({
			path: [start, end],
			geodesic: true,
			strokeColor: style.color,
			strokeOpacity: 0.9,
			strokeWeight: style.strokeWeight,
			zIndex: style.zIndex,
			map,
		})

		polylinesRef.current.push(line)
	}

	return null
}
