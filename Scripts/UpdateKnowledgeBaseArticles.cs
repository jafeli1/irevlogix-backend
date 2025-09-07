using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Text.Json;

namespace irevlogix_backend.Scripts
{
    public class UpdateKnowledgeBaseArticles
    {
        private readonly ApplicationDbContext _context;
        
        public UpdateKnowledgeBaseArticles(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateArticlesAsync()
        {
            var articlesData = GetArticlesData();
            
            Console.WriteLine($"Starting update of {articlesData.Count} knowledge base articles...");
            
            foreach (var articleData in articlesData)
            {
                try
                {
                    var existingArticle = await _context.KnowledgeBaseArticles
                        .FirstOrDefaultAsync(kb => kb.Title == articleData.Title);
                    
                    if (existingArticle != null)
                    {
                        existingArticle.Content = articleData.Content;
                        existingArticle.DateUpdated = DateTime.UtcNow;
                        existingArticle.UpdatedBy = 1; // System update
                        
                        Console.WriteLine($"Updated article: {articleData.Title}");
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Article not found: {articleData.Title}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating article '{articleData.Title}': {ex.Message}");
                }
            }
            
            await _context.SaveChangesAsync();
            Console.WriteLine("All articles updated successfully!");
        }

        private List<ArticleData> GetArticlesData()
        {
            return new List<ArticleData>
            {
                new ArticleData
                {
                    Title = "Asset Intake Procedures",
                    Content = @"Asset intake is the crucial first step in the IT asset disposition (ITAD) and recycling lifecycle, ensuring a secure and efficient process. Proper procedures protect sensitive data, maintain a clear chain of custody, and maximize asset value recovery.

**Key Components of Asset Intake:**

• **Initial Assessment**: Evaluate incoming assets for condition, data sensitivity, and recovery potential
• **Documentation**: Record detailed information including serial numbers, specifications, and client requirements
• **Chain of Custody**: Establish secure tracking from receipt through final disposition
• **Data Security**: Implement immediate security measures for devices containing sensitive information
• **Categorization**: Sort assets by type, condition, and processing requirements

**Intake Process Steps:**

1. **Receipt Verification**: Confirm shipment contents against manifests and purchase orders
2. **Physical Inspection**: Document condition, damage, and completeness of received items
3. **Data Assessment**: Identify devices requiring secure data handling and destruction
4. **Asset Tagging**: Apply unique identifiers for tracking throughout the lifecycle
5. **Initial Sorting**: Group assets by processing requirements and priority levels
6. **Documentation**: Complete intake forms and update inventory management systems

**Quality Control Measures:**

• Verify accuracy of all documentation and asset information
• Ensure proper handling procedures are followed for sensitive equipment
• Confirm compliance with client-specific requirements and industry standards
• Implement double-check procedures for high-value or sensitive assets

**Security Protocols:**

• Secure storage of assets pending processing
• Access control for intake areas and personnel
• Immediate isolation of devices requiring special handling
• Documentation of all personnel interactions with assets

Proper asset intake procedures form the foundation for successful ITAD operations, ensuring compliance, security, and maximum value recovery throughout the entire process."
                },
                new ArticleData
                {
                    Title = "Equipment Handling Guidelines",
                    Content = @"Proper equipment handling is essential for maintaining safety, preserving asset value, and ensuring a responsible recycling process. These guidelines apply to all personnel handling materials and equipment throughout the ITAD lifecycle.

**General Handling Principles:**

• **Safety First**: Always prioritize worker safety and follow established safety protocols
• **Asset Preservation**: Handle equipment to maintain maximum recovery value
• **Environmental Responsibility**: Prevent contamination and environmental damage
• **Documentation**: Record all handling activities and any damage or issues

**Personal Protective Equipment (PPE):**

• Safety glasses or goggles for eye protection
• Cut-resistant gloves for handling sharp edges and components
• Steel-toed boots for foot protection
• Hard hats in designated areas
• Respiratory protection when handling dusty or hazardous materials

**Lifting and Moving Procedures:**

• Use proper lifting techniques to prevent injury
• Employ mechanical aids (dollies, forklifts, hoists) for heavy items
• Team lifting for items exceeding individual capacity
• Clear pathways before moving equipment
• Secure loads during transport

**Equipment-Specific Guidelines:**

**Servers and Network Equipment:**
• Handle by designated lifting points or chassis rails
• Avoid contact with circuit boards and sensitive components
• Use anti-static precautions when required

**Desktop Computers and Laptops:**
• Carry laptops in closed position
• Support desktop units from the bottom
• Avoid pressure on screens and optical drives

**Monitors and Displays:**
• Handle screens with extreme care to prevent cracking
• Use appropriate lifting techniques for CRT monitors
• Protect LCD/LED screens from impact and pressure

**Storage Devices:**
• Handle hard drives and SSDs with care to prevent data recovery issues
• Use anti-static procedures for sensitive components
• Maintain chain of custody for devices requiring secure destruction

**Damage Prevention:**

• Inspect handling areas for hazards before moving equipment
• Use appropriate packaging and cushioning materials
• Avoid stacking incompatible items
• Report and document any damage immediately

Following these guidelines ensures safe, efficient, and responsible handling of all equipment throughout the recycling process."
                },
                new ArticleData
                {
                    Title = "Data Security Protocols",
                    Content = @"Data security is the paramount concern in the IT asset disposition process. Robust protocols ensure that all sensitive and confidential information on retired devices is rendered unrecoverable, protecting both clients and end-users from data breaches.

**Data Security Framework:**

• **Risk Assessment**: Evaluate data sensitivity levels and security requirements
• **Chain of Custody**: Maintain secure tracking of all data-bearing devices
• **Access Control**: Limit personnel access to authorized individuals only
• **Destruction Verification**: Document complete data sanitization and destruction
• **Compliance**: Adhere to industry standards and regulatory requirements

**Device Classification:**

**High Security Devices:**
• Government and military equipment
• Healthcare systems containing PHI
• Financial services equipment
• Legal and professional services devices

**Standard Security Devices:**
• Corporate workstations and laptops
• General business servers
• Network equipment with configuration data

**Low Security Devices:**
• Personal consumer electronics
• Basic office equipment
• Non-data bearing components

**Security Procedures by Classification:**

**High Security Protocol:**
• Immediate isolation upon receipt
• Restricted access to certified personnel only
• Enhanced documentation and tracking
• Witnessed destruction processes
• Certificate of destruction provided

**Standard Security Protocol:**
• Secure storage pending processing
• Standard data sanitization procedures
• Regular documentation and tracking
• Verification of destruction completion

**Physical Security Measures:**

• Secure storage areas with controlled access
• Video surveillance of processing areas
• Locked containers for data-bearing devices
• Visitor access controls and escort requirements
• Regular security audits and assessments

**Personnel Security:**

• Background checks for all data handling personnel
• Security training and certification requirements
• Non-disclosure agreements and confidentiality commitments
• Regular security awareness updates
• Incident reporting procedures

**Documentation Requirements:**

• Chain of custody forms for all devices
• Data sanitization certificates
• Destruction verification records
• Compliance audit trails
• Client reporting and certification

These protocols ensure the highest level of data security throughout the ITAD process, protecting sensitive information and maintaining client trust."
                },
                new ArticleData
                {
                    Title = "Data Sanitization Standards",
                    Content = @"Data sanitization is the process of securely and permanently destroying data from storage media. Adhering to recognized standards is essential for compliance and data protection.

**Acceptable Methods for Data Sanitization:**

**Physical Destruction:**
• Complete physical destruction of storage media
• Shredding, crushing, or disintegration of drives
• Incineration under controlled conditions
• Degaussing for magnetic media

**Cryptographic Erasure:**
• Destruction of encryption keys rendering data unrecoverable
• Applicable to self-encrypting drives (SEDs)
• Verification of key destruction completion

**Overwriting/Wiping:**
• Multiple-pass overwriting of data sectors
• Use of DoD 5220.22-M standard (3-pass minimum)
• NIST SP 800-88 guidelines compliance
• Verification of complete overwrite

**Industry Standards Compliance:**

• **NIST SP 800-88**: Guidelines for Media Sanitization
• **DoD 5220.22-M**: Department of Defense clearing and sanitization standard
• **HIPAA**: Healthcare data protection requirements
• **SOX**: Sarbanes-Oxley financial data requirements
• **GDPR**: European data protection regulation compliance

**Verification Procedures:**

• Post-sanitization verification scans
• Certificate of destruction documentation
• Chain of custody maintenance
• Client reporting and certification
• Audit trail preservation

**Quality Assurance:**

• Regular equipment calibration and testing
• Personnel training and certification
• Process documentation and review
• Continuous improvement implementation

**Documentation Requirements:**

• Detailed sanitization logs
• Method verification records
• Equipment calibration certificates
• Personnel certification records
• Client-specific compliance documentation

**Special Considerations:**

• Solid-state drives (SSD) require specific procedures
• RAID configurations need complete array sanitization
• Encrypted drives require key destruction verification
• Damaged drives may require physical destruction

Proper data sanitization ensures complete data destruction while maintaining compliance with industry standards and regulatory requirements."
                },
                new ArticleData
                {
                    Title = "Processing Lot Management",
                    Content = @"Processing lot management is a core function of a recycling facility that streamlines the processing of diverse materials. It involves grouping incoming materials into manageable batches or ""lots"" to optimize efficiency, maintain quality control, and ensure proper tracking throughout the recycling process.

**Key Principles of Lot Management:**

• **Batch Optimization**: Group similar materials to maximize processing efficiency
• **Quality Control**: Maintain consistent processing standards across batches
• **Traceability**: Track materials from intake through final disposition
• **Resource Planning**: Optimize labor and equipment utilization
• **Compliance**: Ensure regulatory and client requirements are met

**Lot Creation Criteria:**

**Material Type Grouping:**
• Similar equipment types (servers, desktops, laptops)
• Compatible processing requirements
• Consistent material composition
• Similar size and handling characteristics

**Client-Specific Lots:**
• Single-client batches for specialized requirements
• Confidential or high-security materials
• Specific processing or reporting needs
• Custom chain of custody requirements

**Processing Priority Levels:**
• Urgent or time-sensitive materials
• High-value recovery opportunities
• Standard processing timeline items
• Low-priority or bulk materials

**Lot Management Process:**

1. **Lot Planning**: Analyze incoming materials and create processing schedules
2. **Lot Assignment**: Assign materials to appropriate lots based on criteria
3. **Documentation**: Create lot tracking sheets and processing instructions
4. **Resource Allocation**: Assign personnel and equipment to lot processing
5. **Progress Monitoring**: Track processing status and completion rates
6. **Quality Review**: Verify processing standards and output quality
7. **Lot Closure**: Complete documentation and move to next stage

**Tracking and Documentation:**

• Unique lot identification numbers
• Material composition and quantities
• Processing instructions and requirements
• Personnel assignments and responsibilities
• Progress tracking and milestone completion
• Quality control checkpoints and results

**Efficiency Optimization:**

• Batch similar materials to reduce setup time
• Schedule lots to maximize equipment utilization
• Balance workload across processing teams
• Minimize material handling and movement
• Optimize workflow and processing sequences

**Quality Control Measures:**

• Pre-processing inspection and verification
• In-process quality checkpoints
• Post-processing quality review
• Documentation of any issues or deviations
• Corrective action implementation

**Reporting and Analytics:**

• Lot processing performance metrics
• Material recovery rates and yields
• Processing time and efficiency analysis
• Cost tracking and profitability assessment
• Client-specific reporting requirements

Effective lot management ensures efficient processing, maintains quality standards, and provides comprehensive tracking and reporting throughout the recycling process."
                },
                new ArticleData
                {
                    Title = "Inventory Management System",
                    Content = @"A robust inventory management system is essential for any recycling business. It provides real-time visibility and control over all materials and assets, from the moment they are received to their final disposition.

**Core System Functions:**

• **Asset Tracking**: Monitor location and status of all materials throughout the process
• **Real-Time Updates**: Maintain current information on inventory levels and movements
• **Reporting**: Generate comprehensive reports for operations and compliance
• **Integration**: Connect with other business systems for seamless operations
• **Audit Trail**: Maintain complete history of all inventory transactions

**Key Features and Capabilities:**

**Intake Management:**
• Barcode/RFID scanning for rapid data entry
• Automated weight and measurement recording
• Photo documentation and condition assessment
• Client and shipment information linking
• Immediate inventory status updates

**Processing Tracking:**
• Work order generation and management
• Processing stage status updates
• Quality control checkpoint recording
• Material transformation tracking
• Resource utilization monitoring

**Output Management:**
• Finished goods inventory tracking
• Sales order fulfillment
• Shipping and logistics coordination
• Customer delivery confirmation
• Revenue recognition support

**System Benefits:**

**Operational Efficiency:**
• Reduced manual data entry and errors
• Faster processing and turnaround times
• Improved resource planning and utilization
• Enhanced workflow optimization
• Better decision-making through real-time data

**Compliance and Reporting:**
• Automated compliance reporting
• Chain of custody documentation
• Audit trail maintenance
• Regulatory requirement tracking
• Client-specific reporting capabilities

**Financial Management:**
• Accurate cost tracking and allocation
• Revenue optimization through better visibility
• Inventory valuation and accounting
• Profitability analysis by material type
• Budget planning and forecasting support

**Best Practices for System Implementation:**

• Regular data backup and security measures
• User training and system documentation
• Periodic system audits and data validation
• Integration with existing business systems
• Continuous improvement and system updates

**Performance Metrics:**

• Inventory accuracy rates
• Processing cycle times
• System uptime and reliability
• User adoption and satisfaction
• Return on investment measurement

An effective inventory management system transforms recycling operations by providing the visibility, control, and efficiency needed for successful business performance."
                },
                new ArticleData
                {
                    Title = "Vendor Relationship Guidelines",
                    Content = @"Effective vendor relationships are critical for a successful recycling and ITAD program. Partnering with the right vendors ensures a secure, compliant, and profitable end-to-end process.

**Key Criteria for Vendor Selection:**

• **Certifications and Compliance**: Verify industry certifications (R2, e-Stewards, ISO 14001)
• **Security Standards**: Ensure robust data security and chain of custody procedures
• **Financial Stability**: Assess vendor financial health and business continuity
• **Processing Capabilities**: Evaluate technical capabilities and capacity
• **Geographic Coverage**: Consider location and logistics requirements

**Vendor Categories:**

**Downstream Processors:**
• Material refiners and smelters
• Component manufacturers
• Precious metal recovery specialists
• Plastic and metal recyclers

**Service Providers:**
• Transportation and logistics companies
• Data destruction specialists
• Certification and testing services
• Equipment refurbishment partners

**Technology Partners:**
• Software and system providers
• Equipment and machinery suppliers
• Consulting and advisory services
• Training and certification organizations

**Relationship Management Process:**

**Vendor Qualification:**
• Initial assessment and due diligence
• Site visits and capability evaluation
• Reference checks and performance history
• Contract negotiation and agreement
• Ongoing monitoring and review

**Performance Management:**
• Regular performance reviews and scorecards
• Quality metrics and service level agreements
• Issue resolution and corrective action
• Continuous improvement initiatives
• Relationship optimization strategies

**Communication Standards:**

• Regular scheduled meetings and updates
• Clear communication channels and contacts
• Prompt issue escalation and resolution
• Transparent reporting and documentation
• Collaborative planning and forecasting

**Contract Management:**

• Clear terms and conditions
• Service level agreements and penalties
• Pricing structures and payment terms
• Liability and insurance requirements
• Termination and transition procedures

**Risk Management:**

• Vendor diversification strategies
• Backup vendor identification
• Business continuity planning
• Insurance and liability coverage
• Regular risk assessments

**Performance Metrics:**

• Quality and compliance ratings
• On-time delivery and service performance
• Cost competitiveness and value
• Innovation and improvement contributions
• Relationship satisfaction scores

**Best Practices:**

• Maintain multiple qualified vendors for critical services
• Regular vendor performance reviews and feedback
• Collaborative improvement initiatives
• Fair and transparent vendor treatment
• Long-term partnership development

Strong vendor relationships create competitive advantages through improved service, reduced costs, enhanced capabilities, and better risk management throughout the recycling process."
                },
                new ArticleData
                {
                    Title = "Material Recovery Optimization",
                    Content = @"Material recovery optimization is the process of maximizing the quantity and quality of valuable materials recovered from waste streams. This increases profitability and reduces the environmental impact of recycling operations.

**Key Recovery Optimization Strategies:**

• **Advanced Sorting**: Implement sophisticated sorting technologies and techniques
• **Process Efficiency**: Optimize processing workflows and procedures
• **Quality Enhancement**: Improve material purity and grade
• **Market Intelligence**: Understand commodity markets and pricing trends
• **Technology Investment**: Deploy advanced recovery equipment and systems

**Material Categories and Recovery Approaches:**

**Precious Metals:**
• Gold, silver, platinum, and palladium recovery from circuit boards
• Advanced chemical and mechanical separation processes
• High-purity recovery for maximum market value
• Specialized processing for different component types

**Base Metals:**
• Copper, aluminum, steel, and other common metals
• Magnetic and eddy current separation
• Density separation and screening
• Clean material preparation for market

**Rare Earth Elements:**
• Strategic material recovery from specialized components
• Advanced chemical processing techniques
• Market development for recovered materials
• Partnership with specialized processors

**Plastics and Polymers:**
• Identification and sorting by polymer type
• Cleaning and preparation for reprocessing
• Quality enhancement for higher-grade applications
• Market development for recycled plastics

**Recovery Process Optimization:**

**Pre-Processing:**
• Effective dismantling and component separation
• Contamination removal and cleaning
• Size reduction and preparation
• Quality inspection and grading

**Processing Technology:**
• Advanced sorting equipment (optical, magnetic, density)
• Automated dismantling systems
• Chemical recovery processes
• Quality control and testing

**Post-Processing:**
• Material refinement and purification
• Packaging and preparation for market
• Quality certification and documentation
• Logistics and shipping optimization

**Market Optimization:**

• Commodity market analysis and timing
• Customer relationship development
• Contract negotiation and pricing
• Quality specification compliance
• Supply chain optimization

**Performance Metrics:**

• Recovery rates by material type
• Material purity and quality grades
• Processing costs and efficiency
• Market pricing and revenue optimization
• Environmental impact reduction

**Technology and Innovation:**

• Investment in advanced recovery equipment
• Research and development partnerships
• Process improvement initiatives
• Automation and efficiency enhancement
• Emerging technology evaluation

**Continuous Improvement:**

• Regular process review and optimization
• Performance benchmarking and analysis
• Best practice identification and implementation
• Technology upgrade planning
• Market trend analysis and adaptation

Effective material recovery optimization maximizes both economic and environmental benefits, creating sustainable competitive advantages in the recycling industry."
                },
                new ArticleData
                {
                    Title = "Safety Protocols",
                    Content = @"Safety is a top priority in any recycling facility due to the presence of heavy machinery, hazardous materials, and airborne dusts. Strict adherence to safety protocols is essential to protect workers, visitors, and the environment.

**Fundamental Safety Principles:**

• **Prevention First**: Identify and eliminate hazards before they cause harm
• **Personal Responsibility**: Every individual is responsible for their own safety and that of others
• **Continuous Training**: Regular safety education and skill development
• **Incident Reporting**: Prompt reporting and investigation of all safety incidents
• **Continuous Improvement**: Regular review and enhancement of safety procedures

**Personal Protective Equipment (PPE) Requirements:**

**Mandatory PPE for All Personnel:**
• Safety glasses or goggles
• Hard hats in designated areas
• Steel-toed safety boots
• High-visibility clothing
• Cut-resistant gloves

**Task-Specific PPE:**
• Respiratory protection for dusty environments
• Chemical-resistant gloves for hazardous materials
• Fall protection equipment for elevated work
• Hearing protection in high-noise areas
• Specialized protective clothing for specific hazards

**Hazard Identification and Control:**

**Physical Hazards:**
• Moving machinery and equipment
• Sharp edges and protruding objects
• Slips, trips, and falls
• Lifting and ergonomic hazards
• Electrical hazards

**Chemical Hazards:**
• Battery acids and electrolytes
• Cleaning solvents and chemicals
• Refrigerants and coolants
• Toner and ink cartridges
• Mercury and other toxic substances

**Environmental Hazards:**
• Dust and airborne particles
• Noise exposure
• Temperature extremes
• Poor lighting conditions
• Confined spaces

**Emergency Procedures:**

**Fire Emergency:**
• Immediate evacuation procedures
• Fire suppression system activation
• Emergency contact protocols
• Assembly point procedures
• Post-incident reporting

**Medical Emergency:**
• First aid response procedures
• Emergency medical service contact
• Incident documentation
• Follow-up care coordination
• Return-to-work protocols

**Chemical Spill Response:**
• Immediate containment procedures
• Personal protection measures
• Cleanup and disposal protocols
• Incident reporting requirements
• Environmental protection measures

**Training and Certification:**

• New employee safety orientation
• Job-specific safety training
• Regular safety refresher courses
• Equipment operation certification
• Emergency response training

**Safety Management System:**

• Regular safety inspections and audits
• Hazard identification and risk assessment
• Safety performance metrics and reporting
• Incident investigation and corrective action
• Safety committee participation and feedback

**Regulatory Compliance:**

• OSHA standards and requirements
• EPA environmental regulations
• DOT transportation safety rules
• State and local safety codes
• Industry-specific safety standards

**Performance Monitoring:**

• Injury and illness tracking
• Near-miss incident reporting
• Safety training completion rates
• PPE compliance monitoring
• Safety audit results and improvements

A comprehensive safety program protects personnel, reduces liability, improves productivity, and demonstrates commitment to responsible operations."
                },
                new ArticleData
                {
                    Title = "Environmental Impact Assessment",
                    Content = @"An environmental impact assessment for a recycling company measures the positive effects of its operations, such as reduced waste, energy savings, and greenhouse gas reductions. Quantifying these metrics demonstrates environmental stewardship and supports sustainability reporting.

**Key Environmental Benefits:**

• **Waste Diversion**: Preventing materials from entering landfills
• **Resource Conservation**: Reducing demand for virgin materials
• **Energy Savings**: Lower energy consumption compared to primary production
• **Greenhouse Gas Reduction**: Decreased carbon emissions
• **Pollution Prevention**: Reduced air and water pollution

**Assessment Framework:**

**Baseline Establishment:**
• Current environmental performance metrics
• Industry benchmarking and comparison
• Historical trend analysis
• Regulatory compliance status
• Stakeholder expectations and requirements

**Impact Measurement Categories:**

**Material Flow Analysis:**
• Input materials by type and quantity
• Processing efficiency and yield rates
• Output products and byproducts
• Waste generation and disposal
• Material recovery and recycling rates

**Energy and Carbon Footprint:**
• Energy consumption by source and type
• Greenhouse gas emissions (Scope 1, 2, and 3)
• Carbon intensity per unit processed
• Energy efficiency improvements
• Renewable energy utilization

**Water and Air Quality:**
• Water consumption and treatment
• Wastewater generation and quality
• Air emissions and quality monitoring
• Dust and particulate control
• Chemical usage and management

**Waste Management:**
• Hazardous waste generation and disposal
• Non-hazardous waste streams
• Waste minimization initiatives
• Recycling and reuse programs
• Disposal method optimization

**Positive Impact Quantification:**

**Environmental Benefits Calculation:**
• Landfill diversion quantities and impact
• Virgin material substitution rates
• Energy savings compared to primary production
• Greenhouse gas emission reductions
• Water and air pollution prevention

**Life Cycle Assessment:**
• Cradle-to-grave impact analysis
• Comparative assessment with alternatives
• Impact category evaluation
• Sensitivity analysis and uncertainty
• Improvement opportunity identification

**Reporting and Communication:**

**Sustainability Reporting:**
• Annual environmental performance reports
• Third-party verification and certification
• Stakeholder communication and engagement
• Regulatory reporting compliance
• Public disclosure and transparency

**Performance Metrics:**
• Environmental KPIs and targets
• Trend analysis and benchmarking
• Continuous improvement tracking
• Cost-benefit analysis
• Return on environmental investment

**Continuous Improvement:**

• Environmental management system implementation
• Regular performance review and assessment
• Technology upgrade evaluation
• Process optimization initiatives
• Best practice identification and adoption

**Stakeholder Engagement:**

• Customer environmental reporting
• Supplier sustainability requirements
• Community outreach and education
• Regulatory agency collaboration
• Industry association participation

**Certification and Standards:**

• ISO 14001 environmental management
• R2 and e-Stewards certification
• Carbon footprint verification
• Life cycle assessment standards
• Sustainability reporting frameworks

Environmental impact assessment demonstrates the positive contribution of recycling operations to environmental protection and sustainability, supporting business objectives and stakeholder expectations."
                }
            };
        }

        private class ArticleData
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }
    }
}
