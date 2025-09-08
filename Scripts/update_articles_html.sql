
UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Asset intake is the crucial first step in the IT asset disposition (ITAD) and recycling lifecycle, ensuring a secure and efficient process. Proper procedures protect sensitive data, maintain a clear chain of custody, and lay the foundation for maximizing value recovery.</p>

<p><strong>Key Steps for Effective Asset Intake:</strong></p>
<ul>
<li><strong>Pre-Intake Assessment:</strong> Before an asset arrives, conduct an initial assessment. This includes creating a pre-inventory list with details like asset type, serial numbers, and a general description of condition. This helps reconcile the assets upon arrival.</li>
<li><strong>Secure Transportation:</strong> Ensure that IT assets are transported in a secure manner to prevent theft or data breaches. Use tamper-evident seals on containers or pallets and document the transfer with a bill of lading or chain-of-custody form.</li>
<li><strong>Verification and Reconciliation:</strong> Upon arrival at the facility, each asset must be verified against the pre-inventory list. Document any discrepancies, such as missing items or items that were not on the initial list.</li>
<li><strong>Condition Assessment:</strong> Conduct a detailed physical and functional assessment of each asset. Document its condition (e.g., new, used, non-functional, cosmetic damage) to determine its potential for reuse, resale, or recycling.</li>
<li><strong>Data Bearing Device Identification:</strong> A critical step is to identify all data-bearing devices (e.g., hard drives, solid-state drives, mobile phones, memory cards). These devices must be segregated and handled according to strict data security protocols.</li>
<li><strong>Secure Logging and Tagging:</strong> Assign a unique asset tag or ID to each item. Log all key information, including serial number, manufacturer, model, and initial condition, into the inventory management system. This is the start of the secure audit trail for the asset.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 2;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Proper equipment handling is essential for maintaining safety, preserving asset value, and ensuring a responsible recycling process. These guidelines apply to all personnel handling materials and equipment within a recycling facility.</p>

<p><strong>General Guidelines:</strong></p>
<ul>
<li><strong>Use Proper Lifting Techniques:</strong> Bend at the knees and keep your back straight to prevent strains and injuries when lifting heavy items. If an item is too heavy, use lifting equipment such as a forklift or pallet jack.</li>
<li><strong>Wear Appropriate Personal Protective Equipment (PPE):</strong> Always wear required PPE, including safety glasses, hard hats, gloves, and steel-toed boots, to protect against common hazards like falling or flying objects.</li>
<li><strong>Maintain Situational Awareness:</strong> Be aware of your surroundings, especially when operating or working near heavy machinery like forklifts, balers, and compactors. Stay out of their blind spots and use designated walkways.</li>
<li><strong>Safe Handling of Hazardous Materials:</strong> Materials such as batteries, fluorescent light ballasts, and certain chemicals must be handled with extreme care. These items should be placed in designated, properly labeled containers to prevent leaks, contamination, and short circuits.</li>
<li><strong>Secured Transportation of Waste:</strong> Use appropriate packaging to protect equipment and prevent damage during transport. When moving materials, ensure they are properly secured to prevent shifts or spills.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 12;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Data security is the paramount concern in the IT asset disposition process. Robust protocols ensure that all sensitive and confidential information on retired devices is rendered unrecoverable, protecting against data breaches and regulatory penalties.</p>

<p><strong>Core Principles of Data Security:</strong></p>
<ul>
<li><strong>Chain of Custody:</strong> A secure, unbroken chain of custody must be established for every data-bearing device from the moment it leaves the client''s facility until its final disposition. This involves meticulous documentation of every transfer, handling, and destruction event.</li>
<li><strong>Secure Storage:</strong> All data-bearing devices must be stored in a secure, access-controlled area with video surveillance and limited access to authorized personnel only.</li>
<li><strong>Auditable Processes:</strong> All data destruction activities must be meticulously documented and verifiable. Records should include the asset''s serial number, the method of destruction, the date and time, and the name of the individual who performed the destruction.</li>
<li><strong>Data Segregation:</strong> Separate data-bearing devices from non-data-bearing equipment to ensure they are routed through the correct, secure destruction process.</li>
<li><strong>Third-Party Audits:</strong> Partner with vendors who conduct regular third-party security audits (e.g., ISO 27001) to verify the integrity and effectiveness of their data security controls.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 13;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Data sanitization is the process of securely and permanently destroying data from storage media. Adhering to recognized standards is essential for compliance and data protection.</p>

<p><strong>Acceptable Methods for Data Destruction:</strong></p>
<ul>
<li><strong>Data Erasure (Logical Overwrite):</strong> This method uses specialized software to overwrite data on a storage medium multiple times, rendering the original data unrecoverable. The most widely accepted and current standard for this method is NIST SP 800-88 Rev. 1. This is the standard for clear, purge, or destroy data from media. It is important to note that the older DoD 5220.22-M standard is outdated and no longer approved for use by the U.S. Department of Defense.</li>
<li><strong>Physical Destruction:</strong> This method involves the physical destruction of the data storage media, such as shredding, crushing, or pulverizing the device. This process must render the data unreadable and unrecoverable, ensuring there is no chance of data retrieval. This method is a definitive way to destroy data and is often used for devices that cannot be securely wiped.</li>
<li><strong>Degaussing:</strong> This method uses a powerful magnetic field to destroy data on magnetic storage media (e.g., hard drives, tapes).</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 3;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Processing lot management is a core function of a recycling facility that streamlines the processing of diverse materials. It involves grouping incoming materials into manageable batches or "lots" to optimize the workflow and track materials through each stage of the recycling process.</p>

<p><strong>Key Stages of Processing Lot Management:</strong></p>
<ul>
<li><strong>Lot Creation:</strong> Once assets or materials are received, they are sorted and grouped into a new "processing lot." The lot is assigned a unique ID and key information (e.g., material type, weight, source) is logged.</li>
<li><strong>Processing Steps:</strong> Each lot goes through a series of defined steps based on the material type. For electronics, this may include de-manufacturing and component sorting. For plastics, it could involve washing, shredding, and extrusion.</li>
<li><strong>Inventory Tracking:</strong> The lot management system must track the inventory of materials as they are processed. This includes tracking total incoming weight and the final processed weight, which is crucial for mass balance reporting.</li>
<li><strong>Quality Control:</strong> At various stages, materials are tested for class and quality to ensure they meet specifications for their final use or sale.</li>
<li><strong>Disposition and Reporting:</strong> Once processing is complete, the lot is prepared for its final disposition (e.g., sale to a downstream vendor). The system then generates reports, including a certificate of recycling, and financial reports that track all costs and revenues associated with the lot.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 4;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>A robust inventory management system is essential for any recycling business. It provides real-time visibility and control over all materials and assets, from the moment they are received to their final disposition.</p>

<p><strong>Key Features of an Effective System:</strong></p>
<ul>
<li><strong>Real-time Tracking:</strong> The system must provide real-time inventory tracking across all facilities. This allows you to know the exact location and status of every item or lot at any time.</li>
<li><strong>Unique Identification:</strong> Each asset or processing lot should be assigned a unique ID, barcode, or RFID tag to ensure accurate tracking and auditing.</li>
<li><strong>Data-rich Records:</strong> The system should maintain detailed records for each item, including its condition, weight, source, and a complete history of all movements and processing steps.</li>
<li><strong>Automated Updates:</strong> The system should automatically update inventory records at each stage of the process, reducing manual errors and improving efficiency.</li>
<li><strong>Financial Integration:</strong> The inventory system should integrate with accounting software to track costs associated with incoming materials and revenue generated from sales.</li>
<li><strong>Reporting:</strong> It should be able to generate comprehensive reports on inventory levels, turnover, and profitability, which are vital for business optimization.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 14;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Effective vendor relationships are critical for a successful recycling and ITAD program. Partnering with the right vendors ensures a secure, compliant, and profitable end-to-end process.</p>

<p><strong>Key Criteria for Vendor Selection and Management:</strong></p>
<ul>
<li><strong>Certifications:</strong> Always partner with vendors who hold reputable certifications such as R2 (Responsible Recycling), e-Stewards, or ISO certifications (e.g., ISO 9001, ISO 14001, ISO 27001). These certifications provide assurance of secure and environmentally responsible practices.</li>
<li><strong>Transparency and Accountability:</strong> Choose vendors who offer full transparency into their processes. Look for a web portal or system that provides real-time tracking of your assets and access to Certificates of Destruction or Recycling.</li>
<li><strong>Secure Chain of Custody:</strong> The vendor should provide a documented chain of custody that meticulously tracks your assets from pickup to final disposition.</li>
<li><strong>Data Security:</strong> Verify that the vendor''s data destruction methods and protocols align with industry standards like NIST SP 800-88 Rev. 1.</li>
<li><strong>Value Recovery:</strong> Assess a vendor''s resale channels and expertise to ensure they can maximize the value of your assets. A vendor with varied resale channels is preferred for this purpose.</li>
<li><strong>Service Level Agreements (SLAs):</strong> Ensure contracts include clear agreements on response times, reporting frequencies, and a guarantee for data destruction success rates.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 5;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Material recovery optimization is the process of maximizing the quantity and quality of valuable materials recovered from waste streams. This increases profitability and reduces the environmental impact of recycling.</p>

<p><strong>Strategies for Optimization:</strong></p>
<ul>
<li><strong>Source Segregation:</strong> The most effective method is to segregate materials at the source. This prevents contamination and preserves the recycling value of each material type. Use clearly labeled and color-coded bins for different materials like paper, plastic, and metals.</li>
<li><strong>Leverage Technology:</strong> Advanced technologies such as AI-powered sorting robots (e.g., AMP Robotics) and X-ray fluorescence (XRF) analyzers can significantly increase sorting speed and accuracy, enhancing the purity of recovered materials.</li>
<li><strong>Process Audits:</strong> Regularly audit your material recovery processes to identify inefficiencies and bottlenecks. These audits can pinpoint opportunities to increase recovery rates and streamline workflows.</li>
<li><strong>Employee Training:</strong> Develop comprehensive employee training programs to ensure all staff understand proper sorting and handling procedures. Employee participation and awareness are key to effective recycling efforts.</li>
<li><strong>Repurposing and Reuse:</strong> Before recycling, prioritize reusing or repurposing items that are still functional. This extends their lifecycle and conserves resources.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 15;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>Safety is a top priority in any recycling facility due to the presence of heavy machinery, hazardous materials, and airborne dusts. Strict adherence to safety protocols is essential to protect workers and maintain a safe work environment.</p>

<p><strong>Key Safety Protocols:</strong></p>
<ul>
<li><strong>Personal Protective Equipment (PPE):</strong> All workers must use proper PPE, which includes high-visibility clothing, hard hats, safety glasses, gloves, and steel-toed boots. High-visibility clothing is crucial for workers operating near traffic or in busy areas to reduce the risk of accidents.</li>
<li><strong>Hazard Awareness:</strong> Workers must be trained to recognize and handle hazardous materials like batteries, needles, and sharps, which may be improperly discarded.</li>
<li><strong>Safe Lifting and Movement:</strong> Use proper lifting techniques to prevent back injuries and strains. When working near machinery, maintain a safe distance and be aware of their movement. Heavy equipment always has the right of way.</li>
<li><strong>Lockout/Tagout Procedures:</strong> All workers must be trained on and follow lockout/tagout procedures before performing maintenance or repairs on machinery to prevent accidental startup.</li>
<li><strong>Emergency Procedures:</strong> Workers should be familiar with emergency procedures, including evacuation routes, fire safety, and first-aid measures.</li>
<li><strong>Respiratory Protection:</strong> In environments with dust and airborne particulates, a respiratory protection program is necessary. Workers may be required to wear N95 dust masks or half-mask respirators.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 6;

UPDATE "KnowledgeBaseArticles" 
SET "Content" = '<p>An environmental impact assessment for a recycling company measures the positive effects of its operations, such as reduced waste, energy savings, and greenhouse gas reductions. Quantifying these metrics is crucial for reporting to stakeholders and demonstrating a commitment to sustainability.</p>

<p><strong>Key Metrics and Measurement:</strong></p>
<ul>
<li><strong>Waste Diversion:</strong> Measure the total volume or weight of materials recycled versus the amount sent to a landfill. This is a primary metric of success.</li>
<li><strong>Energy and Emissions Reduction:</strong> Calculate energy savings and reduced greenhouse gas emissions (GHG) from recycling. For example, for every metric ton of material recycled, a specific amount of CO2 is saved.</li>
<li><strong>Resource Conservation:</strong> Quantify the amount of natural resources conserved (e.g., trees saved by recycling paper, raw ore saved by recycling metals).</li>
<li><strong>Triple Bottom Line:</strong> The assessment should consider not just environmental benefits but also economic benefits (e.g., reduced disposal costs) and social benefits (e.g., community development).</li>
</ul>

<p><strong>Measurement and Reporting:</strong></p>
<ul>
<li><strong>Data Collection:</strong> Implement a system to track the types and quantities of materials collected and processed. This data can come from weighing stations, inventory records, and vendor reports.</li>
<li><strong>Reporting Tools:</strong> Use environmental calculators and tools (such as those provided by the EPA) to convert raw data into meaningful metrics for your ESG reports.</li>
<li><strong>Documentation:</strong> Maintain comprehensive records of all environmental data and assessments to ensure compliance with legal and regulatory mandates.</li>
</ul>',
    "DateUpdated" = NOW(),
    "UpdatedBy" = 1
WHERE "Id" = 16;

SELECT "Id", "Title", LENGTH("Content") as content_length, "DateUpdated" 
FROM "KnowledgeBaseArticles" 
WHERE "Id" IN (2, 3, 4, 5, 6, 12, 13, 14, 15, 16)
ORDER BY "Id" ASC;
